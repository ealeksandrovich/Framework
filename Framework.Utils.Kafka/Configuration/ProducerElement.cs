namespace Framework.Utils.Kafka.Configuration
{
    using System.Configuration;

    public class ProducerElement : ConfigurationElement
    {
        /// <summary>
        /// Gets a list of servers
        /// </summary>
        [ConfigurationProperty("brokers", IsDefaultCollection = true)]
        internal BrokersCollection Brokers => (BrokersCollection) base["brokers"];

        [ConfigurationProperty("batchDelayTimeMs", IsRequired = true)]
        public int BatchDelayTimeMs
        {
            get
            {
                return (int)this["batchDelayTimeMs"];
            }

            set
            {
                this["batchDelayTimeMs"] = value;
            }
        }
    }
}
