namespace Framework.DataProviders.NHibernate.Conventions
{
    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;
    using FluentNHibernate.Mapping;

    public class NotLazyForAllConvention : IReferenceConvention, IHasManyToManyConvention, IHasManyConvention, IHasOneConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.LazyLoad(Laziness.False);
            instance.Fetch.Join();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Not.LazyLoad();
            instance.Fetch.Subselect();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Not.LazyLoad();
            instance.Fetch.Subselect();
        }

        public void Apply(IOneToOneInstance instance)
        {
            instance.Not.LazyLoad();
            instance.Fetch.Join();
        }
    }
}
