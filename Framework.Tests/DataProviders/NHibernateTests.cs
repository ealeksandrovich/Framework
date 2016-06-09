namespace Framework.Tests.DataProviders
{
    using Database;
    using Framework.DataProviders.NHibernate;
    using Framework.IoC;
    using NUnit.Framework;

    [TestFixture]
    public class NHibernateTests : TestBase
    {
        [Test]
        public void SaveUserTest()
        {
            var userRepository = IocContainerProvider.CurrentContainer.GetInstance<UserRepository>();

            var user = new UserEntity
            {
                Name = "TestUser"
            };
            var result = userRepository.Save(user);

            Assert.AreNotEqual(result, null);
        }
    }

    public class UserRepository : RepositoryBase<UserEntity>
    {
        public UserRepository(NHibernateDataProvider dataProvider) : base(dataProvider)
        {
        }
    }

}

