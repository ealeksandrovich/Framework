namespace Framework.DataProviders.NHibernate
{
    using System;
    using System.Linq;
    using Domain.Entitities;
    using Domain.Filters;
    using global::NHibernate;
    using global::NHibernate.Criterion;
    using global::NHibernate.Engine;

    public abstract class RepositoryBase<TEntity> where TEntity : EntityBase
    {
        protected readonly NHibernateDataProvider DataProvider;

        protected RepositoryBase(NHibernateDataProvider dataProvider)
        {
            this.DataProvider = dataProvider;
        }

        public virtual object Save(TEntity entity)
        {
            if (entity.IsNew)
                return this.DataProvider.Save(entity);
            this.DataProvider.Update(entity);
            return entity.Id;
        }

        public virtual TEntity GetEntityById(Guid id)
        {
            return this.DataProvider.Get<TEntity>(id);
        }

        public virtual void Delete(Guid id)
        {
            TEntity entity = GetEntityById(id);
            if (entity == null)
            {
                return;
            }
            entity.Deleted = true;
            Save(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.Deleted = true;
            Save(entity);
        }

        /// <summary>
        ///     Adds <see cref="paging"/> restrictions to <see cref="query"/>.
        /// </summary>
        protected void AddPaging<T>(IQueryOver<T, T> query, FilterBase paging, bool distinct = false) where T : EntityBase
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (paging == null)
            {
                return;
            }
            if (paging.Take.HasValue)
            {
                query.Take(paging.Take.Value);
            }
            if (paging.Skip.HasValue)
            {
                query.Skip(paging.Skip.Value);
            }

            var rowQuery = distinct ? ToDistinctRowCount(query) : query.ToRowCountQuery();
            paging.TotalCount = rowQuery.FutureValue<int>().Value;
        }

        protected void AddPaging<T>(IQueryable<T> query, FilterBase paging, bool distinct = false) where T : EntityBase
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (paging == null)
            {
                return;
            }
            if (paging.Take.HasValue)
            {
                query = query.Take(paging.Take.Value);
            }
            if (paging.Skip.HasValue)
            {
                query.Skip(paging.Skip.Value);
            }
        }

        /// <summary>
        /// Creates row count query (with unique identifier)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IQueryOver<T> ToDistinctRowCount<T>(IQueryOver<T, T> query) where T : EntityBase
        {
            return query.Clone()
                .Select(Projections.CountDistinct<T>(x => x.Id))
                .ClearOrders()
                .Skip(0)
                .Take(RowSelection.NoValue);
        }
    }
}