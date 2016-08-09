namespace Framework.Utils.Kafka
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using KafkaNet;
    using KafkaNet.Model;

    public class KafkaBootstrapper
    {
        /// <summary>
        /// http://stackoverflow.com/questions/154551/volatile-vs-interlocked-vs-lock
        /// </summary>
        private static volatile KafkaBootstrapper instance;

        private static readonly object SyncRoot = new object();

        private static volatile bool isInitialized;

        private readonly IDictionary<string, Consumer> consumers;

        private Producer client;

        public Producer Client
        {
            get
            {
                if (!isInitialized)
                {
                    throw new InvalidOperationException("KafkaBootstrapper should be initialize before accessing Client property. Call the Init method first.");
                }
                return this.client;
            }
        }

        private KafkaBootstrapper()
        {
            this.consumers = new Dictionary<string, Consumer>();
        }

        public static KafkaBootstrapper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new KafkaBootstrapper();
                            instance.Init();
                        }
                    }
                }

                return instance;
            }
        }

        public Consumer GetConsumer(string name)
        {
            return this.consumers[name];
        }

        private void Init()
        {
            if (!isInitialized)
            {
                lock (SyncRoot)
                {
                    if (!isInitialized)
                    {
                        foreach (var consumerElement in KafkaConfigurationSection.Current.Consumers.Cast<ConsumerElement>())
                        {
                            var consumerBrokers =
                                consumerElement.Brokers.Cast<BrokerElement>().Select(x => new Uri(x.Url)).ToArray();
                            var consumerOptions = new KafkaOptions(consumerBrokers);
                            var consumerRouter = new BrokerRouter(consumerOptions);
                            this.consumers.Add(consumerElement.Name,
                                    new Consumer(new ConsumerOptions(consumerElement.Topic, consumerRouter)));
                        }

                        var producerBrokers =
                            KafkaConfigurationSection.Current.Producer.Brokers.Cast<BrokerElement>()
                                .Select(x => new Uri(x.Url))
                                .ToArray();

                        var producerOptions = new KafkaOptions(producerBrokers);
                        var producerRouter = new BrokerRouter(producerOptions);

                        this.client = new Producer(producerRouter)
                        {
                            BatchDelayTime =
                                TimeSpan.FromMilliseconds(KafkaConfigurationSection.Current.Producer.BatchDelayTimeMs)
                        };

                        isInitialized = true;
                    }
                }
            }
        }
    }
}