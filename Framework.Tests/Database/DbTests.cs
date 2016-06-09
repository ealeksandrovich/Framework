namespace Framework.Tests.Database
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using Domain.Entitities;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Conventions;
    using FluentNHibernate.Mapping;
    using Framework.DataProviders.NHibernate;
    using Framework.DataProviders.NHibernate.Conventions;
    using NHibernate.Tool.hbm2ddl;
    using NUnit.Framework;
    using Configuration = NHibernate.Cfg.Configuration;
    using Environment = NHibernate.Cfg.Environment;

    [TestFixture]
    public class DbTests
    {
        private List<Assembly> Assemblies { get; set; }
        private List<IConvention> ConventionsList { get; set; }

        public DbTests()
        {
            Assemblies = new List<Assembly>
            {
                Assembly.GetAssembly(typeof (UserMap)),
            };
            ConventionsList = new List<IConvention>
            {
                new TableNameConvention(),
                new ColumnNameConvention(),
                new FkNameConvention()
            };
        }

        //http://fluentnhibernate.wikia.com/wiki/Schema_generation
        [Test]
        public void GenerateDbTest()
        {
            GenerateDb(c => new SchemaExport(c).Execute(true, false, false));
        }

        [Test]
        public void GenerateDbToFile()
        {
            var fileName = @"\Tables.sql";
            GenerateDb(c => new SchemaExport(c).SetOutputFile(fileName).Execute(true, false, false));
        }

        [Test]
        public void UpdateDb()
        {
            SchemaUpdate sc = null;
            var cfg = new Configuration()
                .SetProperty(Environment.ConnectionString,
                    ConfigurationManager.ConnectionStrings["Database"].ConnectionString);

            Fluently.Configure(cfg.Configure())
                .Mappings( v => Assemblies.ForEach(
                            a => v.FluentMappings.AddFromAssembly(a).Conventions.Add(ConventionsList.ToArray())))
                .ExposeConfiguration(c => sc = new SchemaUpdate(c))
                .BuildSessionFactory();

            sc.Execute(true, true);

            Assert.IsTrue(sc.Exceptions == null || sc.Exceptions.Count == 0);
        }

        private void GenerateDb(Action<Configuration> cfgAction)
        {
            var cfg = new Configuration()
                .SetProperty(Environment.ConnectionString,
                    ConfigurationManager.ConnectionStrings["Database"].ConnectionString);

            Fluently.Configure(cfg.Configure())
                .Mappings( v => Assemblies.ForEach(
                            a => v.FluentMappings.AddFromAssembly(a).Conventions.Add(ConventionsList.ToArray())))
                .ExposeConfiguration(cfgAction)
                .BuildSessionFactory();
        }
    }


    public class UserEntity : EntityBase
    {
        public virtual string Name { get; set; }
    }

    public class UserMap : ClassMap<UserEntity>
    {
        public UserMap()
        {
            this.MapBase();
            Map(x => x.Name);
        }
    }
}

