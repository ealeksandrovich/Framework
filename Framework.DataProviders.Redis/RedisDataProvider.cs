namespace Framework.DataProviders.Redis
{
    using StackExchange.Redis;

    public class RedisDataProvider
    {
        private readonly ConnectionMultiplexer connectionMultiplexer;
        private readonly int db;

        public RedisDataProvider(string configuration, int db)
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(configuration);

            this.connectionMultiplexer = ConnectionMultiplexer.Connect(options);
            this.db = db;
        }

        public IDatabase GetDb()
        {
            return this.connectionMultiplexer.GetDatabase(this.db);
        }
    }
}
