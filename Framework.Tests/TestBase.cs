namespace Framework.Tests
{
    using Framework.DataProviders.NHibernate;
    using Framework.IoC;
    using NUnit.Framework;

    public class TestBase
    {
        static TestBase()
        {
            IocContainerProvider.InitIoc();
        }
        [SetUp]
        public virtual void SetUp()
        {
            IocContainerProvider.CurrentContainer.GetInstance<NHibernateDataProvider>().BeginTransaction();
        }

        [TearDown]
        public virtual void TearDown()
        {
            IocContainerProvider.CurrentContainer.GetInstance<NHibernateDataProvider>().CommitTransaction();
        }
    }
}
