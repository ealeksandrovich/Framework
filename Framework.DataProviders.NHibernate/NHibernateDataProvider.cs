namespace Framework.DataProviders.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Conventions;
    using Domain.Entitities;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Conventions;
    using global::NHibernate;
    using global::NHibernate.Cfg;
    using global::NHibernate.Context;
    using global::NHibernate.Criterion;
    using global::NHibernate.Linq;
    using Interceptors;
    using Environment = global::NHibernate.Cfg.Environment;

    public class NHibernateDataProvider
    {
        private const string MappingsProjectTemplate = ".*.Data";

        private readonly ISessionFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        /// <param name="currentSessionContextType"></param>
        /// <param name="additionalAssemblyNames"></param>
        public NHibernateDataProvider(string connectionString, Type currentSessionContextType = null, IEnumerable<AssemblyName> additionalAssemblyNames = null)
        {
            //https://github.com/jagregory/fluent-nhibernate/wiki/Conventions
            var conventionsList = new List<IConvention>
            {
                new TableNameConvention(), new ColumnNameConvention(), new FkNameConvention()
            };

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName())
                .Where(a => new Regex(MappingsProjectTemplate).IsMatch(a.Name))
                .ToList();

            if (additionalAssemblyNames != null)
            {
                assemblies = assemblies.Union(additionalAssemblyNames).ToList();
            }

            assemblies.ForEach(x => Trace.WriteLine(x.FullName));

            if (currentSessionContextType == null)
            {
                currentSessionContextType = typeof(NhSessionContext);
            }

            var configuration = new Configuration()
                .SetProperty(Environment.ConnectionString, connectionString)
                .SetInterceptor(new TrackChangesInterceptor());

            this.factory = Fluently.Configure(configuration.Configure())
                .Mappings(v => assemblies.ForEach(a => v.FluentMappings.AddFromAssembly(Assembly.Load(a))
                    .Conventions.Add(conventionsList.ToArray())))
                .CurrentSessionContext(currentSessionContextType.AssemblyQualifiedName)
                .BuildSessionFactory();
        }

        public ISession Session
        {
            get { return this.OpenSession(); }
        }

        public ISession OpenSession()
        {
            if (CurrentSessionContext.HasBind(this.factory))
            {
                return this.factory.GetCurrentSession();
            }

            ISession session = this.factory.OpenSession();
            session.FlushMode = FlushMode.Never;
            
            CurrentSessionContext.Bind(session);

            return session;
        }

        public void CloseSession()
        {
            ISession session = CurrentSessionContext.Unbind(this.factory);
            if (session != null && session.IsOpen)
            {
                try
                {
                    if (session.Transaction != null && session.Transaction.IsActive)
                    {
                        session.Transaction.Rollback();
                    }
                }
                finally
                {
                    session.Close();
                    session.Dispose();
                }
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            this.Session.BeginTransaction(isolationLevel);
        }

        public void BeginTransaction()
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitTransaction()
        {
            try
            {
                if (this.Session != null && this.Session.Transaction.IsActive)
                {
                    this.Session.Transaction.Commit();
                }
            }
            catch
            {
                this.RollbackTransaction();
                throw;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                this.Session.Transaction.Rollback();
            }
            finally
            {
                this.CloseSession();
            }
        }

        /// <summary>
        /// Completely clears the session. Evict all loaded instances and cancel pending saves, updates and deletes.
        /// </summary>
        public void Clear()
        {
            this.Session.Clear();
        }

        /// <summary>
        /// Creates root criteria for current session.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="ICriteria" />.
        /// </returns>
        public ICriteria CreateCriteria<T>() where T : EntityBase
        {
            return this.Session.CreateCriteria(typeof(T));
        }

        /// <summary>
        /// Saves entity to database.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object Save<T>(T entity) where T : EntityBase
        {
            object id = this.Session.Save(entity);

            this.Session.Flush();
            return id;
        }

        /// <summary>
        /// Saves entity to database.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        public void Delete<T>(T entity) where T : EntityBase
        {
            this.Session.Delete(entity);
            this.Session.Flush();
        }

        /// <summary>
        /// The query over.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IQueryOver" />.
        /// </returns>
        public IQueryOver<T, T> QueryOver<T>() where T : EntityBase
        {
            var query = this.Session.QueryOver<T>();
            return query;
        }

        public IQueryable<T> Query<T>() where T : EntityBase
        {
            return this.Session.Query<T>();
        }

        public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : EntityBase
        {
            var query = this.Session.QueryOver(alias);

            return query;
        }

        public IQueryOver<T, T> QueryOver<T>(QueryOver<T> detachedQuery) where T : EntityBase
        {
            var query = detachedQuery.GetExecutableQueryOver(Session);

            return query;
        }

        public void Unproxy<T>(T enity) where T : EntityBase
        {
            this.Session.GetSessionImplementation().PersistenceContext.Unproxy(enity);
        }

        /// <summary>
        ///     Return the persistent instance of the <see cref="T" /> entity with the given identifier, or null if there is no
        ///     such persistent instance. (If the instance, or a proxy for the instance, is already associated with the session,
        ///     return that instance or proxy.)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="id">an identifier</param>
        /// <returns>a persistent instance or null</returns>
        public T Get<T>(object id) where T : EntityBase
        {
            return this.Session.Get<T>(id);
        }

        /// <summary>
        /// Gets new stateless session. Can be used only in specific cases.
        /// </summary>
        /// <returns></returns>
        public IStatelessSession OpenStatelessSession()
        {
            return this.factory.OpenStatelessSession();
        }

        /// <summary>
        /// Execute raw sql query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ISQLQuery CreateSqlQuery(string query)
        {
            return this.Session.CreateSQLQuery(query);
        }

        /// <summary>
        /// Saves or updates entity to database.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public void SaveOrUpdate<T>(T entity) where T : EntityBase
        {
            this.Session.SaveOrUpdate(entity);
            this.Session.Flush();
        }

        public void Update<T>(T entity) where T : EntityBase
        {
            this.Session.Update(entity);
            this.Session.Flush();
        }

        public void Merge<T>(T entity) where T : EntityBase
        {
            this.Session.Merge(entity);
            this.Session.Flush();
        }
    }
}