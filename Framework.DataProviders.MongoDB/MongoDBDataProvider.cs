namespace Framework.DataProviders.MongoDB
{
    using global::MongoDB.Driver;

    /// <summary>
    /// Represents MongoDb data provider using <see cref="https://docs.mongodb.com/ecosystem/drivers/csharp/"/>. 
    /// As a simple way <see cref="https://github.com/RobThree/MongoRepository"/> can be used.
    /// </summary>
    public class MongoDBDataProvider
    {
        private readonly IMongoDatabase database;

        public MongoDBDataProvider(string connectionString)
        {
            //http://mongodb.github.io/mongo-csharp-driver/2.2/reference/driver/connecting/
            var client = new MongoClient(connectionString);

            var databaseName = MongoUrl.Create(connectionString).DatabaseName;

            this.database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return this.database.GetCollection<T>(collectionName);
        }
    }
}
