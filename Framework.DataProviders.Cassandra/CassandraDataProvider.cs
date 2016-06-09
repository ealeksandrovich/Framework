namespace Framework.DataProviders.Cassandra
{
    using System.Collections.Generic;
    using System.Linq;
    using global::Cassandra;
    using global::Cassandra.Mapping;

    /// <summary>
    /// https://github.com/datastax/csharp-driver
    /// </summary>
    public class CassandraDataProvider
    {
        public ISession Session { get; }

        /// <summary>
        /// http://datastax.github.io/csharp-driver/features/components/mapper/
        /// </summary>
        public Mapper Mapper { get { return new Mapper(Session); } }

        public CassandraDataProvider(string address, string keyspace, IList<Mappings> mappings)
        {
            Cluster cluster = Cluster.Builder().AddContactPoint(address).Build();

            if (mappings != null && mappings.Count > 0)
            {
                MappingConfiguration.Global.Define(mappings.ToArray());
            }

            Session = cluster.Connect(keyspace.ToLower());
        }
    }
}
