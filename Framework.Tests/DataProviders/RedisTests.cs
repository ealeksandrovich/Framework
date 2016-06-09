namespace Framework.Tests.DataProviders
{
    using Framework.DataProviders.Redis;
    using NUnit.Framework;
    using StackExchange.Redis;

    [TestFixture]
    public class RedisTests
    {
        private readonly IDatabase database;

        public RedisTests()
        {
            var provider = new RedisDataProvider("52.36.99.52", 0);
            this.database = provider.GetDb();
        }

        [Test]
        public void SetTest()
        {
            var result = this.database.StringSet("testKey", "testValue");

            Assert.IsTrue(result);
        }

        [Test]
        public void GetTest()
        {
            var value = this.database.StringGet("testKey");

            Assert.AreEqual("testValue", value.ToString());
        }

    }
}