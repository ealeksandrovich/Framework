namespace Framework.DataProviders.Aerospike
{
    using global::Aerospike.Client;

    public class AerospikeDataProvider
    {
        public const int DefaultPort = 3000;

        public AerospikeClient Client { get; }

        public AerospikeDataProvider(string hostName) : this(hostName, DefaultPort)
        {
        }


        /// <summary>
        /// http://www.aerospike.com/docs/client/csharp/usage/connect_sync.html
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public AerospikeDataProvider(string hostName, int port)
        {
            Client = new AerospikeClient(hostName, port);
        }

        public void Close()
        {
            Client.Close();
        }
    }
}