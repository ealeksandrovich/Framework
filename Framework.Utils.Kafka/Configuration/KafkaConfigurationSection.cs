namespace Framework.Utils.Kafka.Configuration
{
    using System.Configuration;

    public class KafkaConfigurationSection : ConfigurationSection
    {
        protected KafkaConfigurationSection()
        {
        }

        /// <summary>
        /// Gets the current kafka config section.
        /// </summary>
        public static KafkaConfigurationSection Current => (KafkaConfigurationSection)ConfigurationManager.GetSection("kafka");

        /// <summary>
        /// Gets a list of consumers
        /// </summary>
        [ConfigurationProperty("consumers", IsDefaultCollection = true)]
        internal ConsumersCollection Consumers => (ConsumersCollection)(base["consumers"]);
        
        [ConfigurationProperty("producer", IsRequired = false)]
        public ProducerElement Producer
        {
            get
            {
                return (ProducerElement)this["producer"];
            }

            set
            {
                this["producer"] = value;
            }
        }
    }
}
