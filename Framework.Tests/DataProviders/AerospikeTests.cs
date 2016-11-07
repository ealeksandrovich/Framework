namespace Framework.Tests.DataProviders
{
    using System;
    using Aerospike.Client;
    using Framework.DataProviders.Aerospike;
    using NUnit.Framework;

    [TestFixture]
    public class AerospikeTests
    {
        private const string Namespace = "test";//default in Aerospike
        private const string SetName = "Users";
        private const string BinName = "User";

        private readonly AerospikeDataProvider provider;

        public AerospikeTests()
        {
            this.provider = new AerospikeDataProvider("192.168.56.101", 3000);
        }

        /// <summary>
        /// http://www.aerospike.com/docs/client/csharp/usage/kvs/write.html
        /// </summary>
        [Test]
        public void PutTest()
        {
            var user = new AerospikeUser
            {
                UserId = new Guid("6B16F333-0B46-4057-9FA6-7CF630A0A0BC"),
                Name = "AerospikeUser"
            };

            var key = new Key(Namespace, SetName, "6B16F333-0B46-4057-9FA6-7CF630A0A0BC");

            var bin = new Bin(BinName, user);

            this.provider.Client.Put(null, key, bin);
        }

        /// <summary>
        /// http://www.aerospike.com/docs/client/csharp/usage/kvs/read.html
        /// </summary>
        [Test]
        public void GetTest()
        {
            var key = new Key(Namespace, SetName, "6B16F333-0B46-4057-9FA6-7CF630A0A0BC");

            Record record = this.provider.Client.Get(null, key);

            Assert.IsNotNull(record);
            var user = record.GetValue(BinName) as AerospikeUser;
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Name, "AerospikeUser");
        }
    }

    [Serializable]
    public class AerospikeUser
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
