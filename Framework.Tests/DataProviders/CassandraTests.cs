namespace Framework.Tests.DataProviders
{
    using System;
    using System.Collections.Generic;
    using Cassandra.Mapping;
    using Framework.DataProviders.Cassandra;
    using NUnit.Framework;

    [TestFixture]
    public class CassandraTests
    {
        private readonly CassandraDataProvider provider;

        public CassandraTests()
        {
            //NOTE http://stackoverflow.com/questions/18724334/cant-connect-to-cassandra-nohostavailableexception
            //NOTE https://www.youtube.com/watch?v=fspXzjwfii0
            //TODO create "CassandraTest"  key space first of all using the following command: create keyspace "CassandraTest" with replication = {'class': 'SimpleStrategy', 'replication_factor': 3};
            this.provider = new CassandraDataProvider("52.36.99.52", "CassandraTest", new List<Mappings> { new MyMappings() });
        }

        [Test]
        public void CreateDb()
        {
            this.provider.Session.Execute("CREATE TABLE user (id uuid, name text, PRIMARY KEY (id));");
        }

        [Test]
        public void InserTest()
        {
            var newUser = new CassandraUser { UserId = Guid.NewGuid(), Name = "SomeNewUser" };
            this.provider.Mapper.Insert(newUser);
        }
    }

    public class MyMappings : Mappings
    {
        public MyMappings()
        {
            // Define mappings in the constructor of your class
            // that inherits from Mappings
            For<CassandraUser>()
                .TableName("users")
                .PartitionKey(u => u.UserId)
                .Column(u => u.UserId, cm => cm.WithName("id"))
                .Column(u => u.Name, cm => cm.WithName("name"));
        }
    }

    public class CassandraUser
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
