namespace Framework.DataProviders.Cassandra
{
    using System;
    using Domain.Entitities;

    /// <summary>
    /// http://datastax.github.io/csharp-driver/features/components/mapper/
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class RepositoryBase<TEntity> where TEntity : EntityBase
    {
        protected readonly CassandraDataProvider DataProvider;

        protected RepositoryBase(CassandraDataProvider dataProvider)
        {
            this.DataProvider = dataProvider;
        }

        public virtual void Save(TEntity entity)
        {
            if (entity.IsNew)
                this.DataProvider.Mapper.Insert(entity);
            else
                this.DataProvider.Mapper.Update(entity);
        }

        public virtual TEntity GetEntityById(Guid id)
        {
            return this.DataProvider.Mapper.Single<TEntity>("WHERE Id = ?", id);
        }

        public virtual void Delete(Guid id)
        {
            this.DataProvider.Mapper.Delete<TEntity>("WHERE Id = ?", id);
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            this.DataProvider.Mapper.Delete(entity);
        }
    }
}