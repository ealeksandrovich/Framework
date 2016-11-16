namespace Framework.WebApi.Configuration
{
    using System.Configuration;

    public class ServiceContainerSection : ConfigurationSection
    {
        protected ServiceContainerSection()
        {
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        public static ServiceContainerSection Current => (ServiceContainerSection)ConfigurationManager.GetSection("serviceContainer");

        [ConfigurationProperty("serviceName", IsRequired = true)]
        public string ServiceName => (string)this["serviceName"];

        [ConfigurationProperty("description", IsRequired = true)]
        public string Description => (string)this["description"];

        [ConfigurationProperty("displayName", IsRequired = false)]
        public string DisplayName => (string)this["displayName"];

        [ConfigurationProperty("instanceName", IsRequired = false)]
        public string InstanceName => (string)this["instanceName"];
    }
}
