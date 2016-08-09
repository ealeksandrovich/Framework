namespace Framework.Utils.Kafka.Configuration
{
    using System.Configuration;

    public class ConsumerElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the name of consumer.
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name => (string)base["name"];

        /// <summary>
        /// Gets the topic of consumer
        /// </summary>
        [ConfigurationProperty("topic", IsRequired = true)]
        public string Topic => (string)base["topic"];
        
        /// <summary>
        /// Gets a list of servers
        /// </summary>
        [ConfigurationProperty("brokers", IsDefaultCollection = true)]
        internal BrokersCollection Brokers => (BrokersCollection)(base["brokers"]);
    }
}
