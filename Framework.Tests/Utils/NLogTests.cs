namespace Framework.Tests.Utils
{
    using System;
    using System.Diagnostics;
    using Framework.Utils.Kafka;
    using KafkaNet;
    using KafkaNet.Common;
    using KafkaNet.Model;
    using KafkaNet.Protocol;
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

        [Test]
        public void KafkaTest()
        {
            Logger.Error("This is a test error message from Framework.Tests.Utils.NLogTests.KafkaTest");

            var consumer = KafkaBootstrapper.Instance.GetConsumer("test");
            foreach (var data in consumer.Consume())
            {
                Trace.WriteLine(data.Value.ToUtf8String());
            }
        }
        [Test]
        public void KafkaBaseTest()
        {
            var options = new KafkaOptions(new Uri("http://localhost:9092"));
            //var options = new KafkaOptions(new Uri("http://52.36.99.52:9092"));
            var router = new BrokerRouter(options);
            var client = new Producer(router);

            client.SendMessageAsync("TestHarness", new[] { new Message("Test") }).Wait();

            var consumer = new Consumer(new ConsumerOptions("TestHarness", router));
            foreach (var data in consumer.Consume())
            {
                Trace.WriteLine(data.Value.ToUtf8String());
            }
        }
    }
}