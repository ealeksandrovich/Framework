namespace Framework.Tests.IoC
{
    using System.Configuration;
    using System.Reflection;
    using Database;
    using DataProviders;
    using Framework.DataProviders.NHibernate;
    using Framework.IoC;
    using SimpleInjector;

    public class TestModule: IIocModule
    {
        public void Register(Container container)
        {
            container.RegisterSingleton(typeof(NHibernateDataProvider), new NHibernateDataProvider(ConfigurationManager.ConnectionStrings["Database"].ConnectionString,
                    null, new []
                    {
                        Assembly.GetAssembly(typeof (UserMap)).GetName()
                    }));

            container.RegisterSingleton<UserRepository>();
        }
    }
}
