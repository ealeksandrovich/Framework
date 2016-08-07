namespace Framework.Tests.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Framework.DataProviders.MongoDB;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using NUnit.Framework;

    /// <summary>
    /// http://mongodb.github.io/mongo-csharp-driver/2.2/getting_started/quick_tour/
    /// http://mongodb.github.io/mongo-csharp-driver/2.2/reference/driver/definitions/
    /// http://mongodb.github.io/mongo-csharp-driver/2.2/reference/driver/expressions/
    /// To view the results use http://www.mongoclient.com/
    /// </summary>
    [TestFixture]
    public class MongoDBTests
    {
        private readonly MongoDBDataProvider provider;

        /// <summary>
        /// 
        /// </summary>
        public MongoDBTests()
        {
            this.provider = new MongoDBDataProvider("mongodb://52.36.99.52:27017/test");
        }

        [Test]
        public void InsertOneTest()
        {
            var collection = this.provider.GetCollection<BsonDocument>("test");
            var bson = new BsonDocument("Name", "TestName");
            collection.InsertOne(bson);

            var list = collection.Find(new BsonDocument("Name", "TestName")).ToList();

            Assert.AreEqual(list.First()["Name"].AsString, "TestName");
        }

        [Test]
        public void DeleteOneTest()
        {
            var collection = this.provider.GetCollection<BsonDocument>("test");

            var result = collection.DeleteOne(new BsonDocument("Name", "TestName"));

            Assert.AreEqual(result.DeletedCount, 1);
        }

        [Test]
        public void CreateUserTest()
        {
            var collection = this.provider.GetCollection<User>("User");
            var user = new User
            {
                FirstName = "Eugeny",
                LastName = "Aleksandrovich"
            };
           
            collection.InsertOne(user);
        }

        [Test]
        public void CreateManyUserTest()
        {
            var collection = this.provider.GetCollection<User>("User");
            var users = new List<User>
            {
                new User
                {
                    FirstName = "Tatyana",
                    LastName = "Aleksandrovich"
                },
                new User
                {
                    FirstName = "Vitaly",
                    LastName = "Aleksandrovich"
                }
            };
            collection.InsertMany(users);
        }

        [Test]
        public void UserListTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var filter = Builders<User>.Filter.Empty;

            var list = collection.Find(filter).ToList();

            Assert.AreEqual(list.Count, 3);
        }

        [Test]
        public void UserFilterTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var filter = Builders<User>.Filter.Eq(x => x.FirstName, "Eugeny");

            var list = collection.Find(filter).ToList();

            Assert.AreEqual(list.Count, 1);
        }

        [Test]
        public void UserFilterViaExpresionTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var list = collection.Find(x => x.FirstName == "Eugeny").ToList();

            Assert.AreEqual(list.Count, 1);
        }

        [Test]
        public void UserProjectionTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var projection = Builders<User>.Projection.Include(x => x.FirstName).Exclude("_id");

            var list = collection.Find(x => x.FirstName == "Eugeny").Project(projection).ToList();

            Assert.AreEqual(list.First().ElementCount, 1);

            Assert.AreEqual(list.First()["FirstName"].AsString, "Eugeny");
        }

        [Test]
        public void UserProjectionExpressionTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var projection = Builders<User>.Projection.Expression(x => new ShortUser { FirstName = x.FirstName, LastName = x.LastName });

            var list = collection.Find(x => x.FirstName == "Eugeny").Project(projection).ToList();

            Assert.AreNotEqual(list.FirstOrDefault(), null);

            Assert.AreEqual(list.First().FirstName, "Eugeny");
        }

        [Test]
        public void UserSortingTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var filter = Builders<User>.Filter.Empty;

            var sort = Builders<User>.Sort.Descending(x=> x.FirstName);

            var user = collection.Find(filter).Sort(sort).First();

            Assert.AreEqual(user.FirstName, "Vitaly");
        }

        [Test]
        public void UserUpdateTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var updateDefinition = Builders<User>.Update.Set(x=> x.LastName, "Pavlukanets");

            var result = collection.UpdateOne(x => x.FirstName == "Tatyana", updateDefinition);

            Assert.AreEqual(result.ModifiedCount, 1);
        }

        [Test]
        public void UserDeleteTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var result = collection.DeleteMany(x => x.FirstName == "Tatyana");

            Assert.AreEqual(result.DeletedCount, 1);
        }

        [Test]
        public void UserDeleteAllTest()
        {
            var collection = this.provider.GetCollection<User>("User");

            var filter = Builders<User>.Filter.Empty;

            var result = collection.DeleteMany(filter);

            Assert.AreEqual(result.DeletedCount, 2);
        }
    }


    internal class User: ShortUser
    {
        public Guid Id { get; set; }
    }

    internal class ShortUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}