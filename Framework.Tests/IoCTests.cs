namespace Framework.Tests
{
    using Framework.IoC;
    using NUnit.Framework;
    using SimpleInjector;

    public class IoCTests
    {
        [Test]
        public virtual void InitTest()
        {
            IocContainerProvider.InitIoc();

            var obj = IocContainerProvider.CurrentContainer.GetInstance<TestClass>();

            Assert.AreNotEqual(1, obj.GetOne());
        }
    }

    public class TestModule: IIocModule
    {
        public void Register(Container container)
        {
            container.RegisterSingleton<TestClass>();
        }
    }

    public class TestClass
    {
        public int GetOne()
        {
            return 1;
        }
    }
}
