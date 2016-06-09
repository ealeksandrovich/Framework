namespace Framework.Tests.DataProviders
{
    using System.Configuration;
    using Framework.DataProviders.AmazonS3;
    using NHibernate.Util;
    using NUnit.Framework;

    [TestFixture]
    public class AmazonS3Tests
    {
        private readonly AmazonS3DataProvider provider;
        private readonly string accessKeyId;
        private readonly string secretKey;

        public AmazonS3Tests()
        {
            this.accessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
            this.secretKey = ConfigurationManager.AppSettings["secretKey"];
            this.provider = new AmazonS3DataProvider(this.accessKeyId, this.secretKey, ConfigurationManager.AppSettings["regionEndpointString"]);
        }

        [Test]
        public void RegisterProfileTest()
        {
            Amazon.Util.ProfileManager.RegisterProfile("test-aws-profile", this.accessKeyId, this.secretKey);
        }
        
        [Test]
        public void CreateBucketTest()
        {
            this.provider.CreateBucket("awsgeshasecondbucket");
        }

        [Test]
        public void CreateFolderTest()
        {
            this.provider.CreateFolder("awsgeshasecondbucket", "test1");
        }

        [Test]
        public void CreateSubFolderTest()
        {
            this.provider.CreateFolder("awsgeshasecondbucket", "test1/test2");
        }


        [Test]
        public void UploadFileTest()
        {
            this.provider.UploadFile("awsgeshasecondbucket", @"C:/test.txt");
        }


        [Test]
        public void DownloadFileTest()
        {
            using (var stream = this.provider.DownloadFile("awsgeshasecondbucket", @"test.txt"))
            {
                Assert.AreNotEqual(stream.Length, 0);
            }
        }

        [Test]
        public void ListTest()
        {
            var result = this.provider.List("awsgeshasecondbucket");
            Assert.IsTrue(result.Any());
        }

        [Test]
        public void CopyTest()
        {
            this.provider.Copy("awsgeshasecondbucket", "test.txt", "test_copy.txt");
        }

        [Test]
        public void DeleteTest()
        {
            this.provider.Delete("awsgeshasecondbucket", "test_copy.txt");
        }
    }
}