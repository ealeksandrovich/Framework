namespace Framework.Tests.Utils
{
    using NLog;
    using NUnit.Framework;

    [TestFixture]
    public class NLogTests
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void ElasticSearchTest()
        {
            Logger.Error("This is a test error message from Framework.Tests.Utils.NLogTests.ElasticSearchTest");
        }
    }
}