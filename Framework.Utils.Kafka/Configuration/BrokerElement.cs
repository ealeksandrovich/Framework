namespace Framework.Utils.Kafka.Configuration
{
    using System.Configuration;

    public class BrokerElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name => (string)base["name"];

        /// <summary>
        /// Gets the name.
        /// </summary>
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url => (string)base["url"];
    }
}
