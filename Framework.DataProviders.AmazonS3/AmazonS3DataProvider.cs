namespace Framework.DataProviders.AmazonS3
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;
    using NLog;

    /// <summary>
    /// http://docs.aws.amazon.com/AmazonS3/latest/dev/ObjectOperations.html
    /// 
    /// </summary>
    public class AmazonS3DataProvider
    {
        private const string Delimiter = "/";

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public TransferUtility TransferUtility { get; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="accessKeyId">The AWS Access Key ID.</param>
        /// <param name="secretKey">The AWS Secret Access Key.</param>
        /// <param name="regionEndpointString">The region to configure the transfer utility for. Should be one of the following list: USEast1, USWest1 and so on <see cref="RegionEndpoint"/> </param>
        public AmazonS3DataProvider(string accessKeyId, string secretKey, string regionEndpointString)
        {
            var endpointField = typeof(RegionEndpoint).GetField(regionEndpointString, BindingFlags.Static | BindingFlags.Public);

            if (string.IsNullOrWhiteSpace(regionEndpointString) || endpointField == null)
                throw new Exception("regionEndpointString is required.");

            var regionEndpoint = (RegionEndpoint)endpointField.GetValue(null);

            TransferUtility = new TransferUtility(accessKeyId, secretKey, regionEndpoint);
        }

        /// <summary>
        /// Creates a bucket.
        /// </summary>
        public void CreateBucket(string bucketName, List<S3Grant> grants = null)
        {
            try
            {
                PutBucketRequest putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName
                };

                if (grants != null)
                {
                    putBucketRequest.Grants = grants;
                }

                TransferUtility.S3Client.PutBucket(putBucketRequest);
            }
            catch (AmazonS3Exception e)
            {
                LogAmazonS3Exception("Bucket creation has failed.", e);
                throw;
            }
        }

        /// <summary>
        /// Creates a folder/subfolder in the bucket. 
        /// </summary>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="folderName">The folder name. To create subfolder put the whole path seprated by "/", f.e this-is-a-subfolder/another-subfolder</param>
        public void CreateFolder(string bucketName, string folderName)
        {
            try
            {
                var folderKey = string.Concat(folderName, Delimiter);

                PutObjectRequest folderRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = folderKey,
                    InputStream = new MemoryStream(new byte[0])
                };

                TransferUtility.S3Client.PutObject(folderRequest);
            }
            catch (AmazonS3Exception e)
            {
                LogAmazonS3Exception("Bucket creation has failed.", e);
                throw;
            }
        }

        /// <summary>
        /// Uploads the file to the storage.
        /// </summary>
        public void UploadFile(string bucketName, string filePath)
        {
            try
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    FilePath = filePath,
                    Key = Path.GetFileName(filePath),
                    PartSize = 5*1024*1024,
                    StorageClass = S3StorageClass.ReducedRedundancy,
                    AutoCloseStream = true,
                    CannedACL = S3CannedACL.PublicRead
                };

                this.TransferUtility.Upload(request);
            }
            catch (AmazonS3Exception e)
            {
                LogAmazonS3Exception("File upload has failed.", e);
                throw;
            }
        }

        /// <summary>
        /// Download the file from the storage.
        /// </summary>
        public Stream DownloadFile(string bucketName, string key)
        {
            try
            {
                GetObjectRequest getObjectRequest = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                GetObjectResponse response = TransferUtility.S3Client.GetObject(getObjectRequest);

                return response.ResponseStream;
            }
            catch (AmazonS3Exception e)
            {
                LogAmazonS3Exception("File download has failed.", e);
            }

            return null;
        }

        /// <summary>
        /// Gaets list of items from a bucket.
        /// </summary>
        public IList<S3Object> List(string bucketName)
        {
            try
            {
                ListObjectsRequest listObjectsRequest = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                ListObjectsResponse response = TransferUtility.S3Client.ListObjects(listObjectsRequest);

                return response.S3Objects;
            }
            catch (AmazonS3Exception e)
            {
                LogAmazonS3Exception("Getting the list of objects has failed.", e);
                throw;
            }
        }

        /// <summary>
        /// Copies the source content to the specified destination on the storage.
        /// </summary>
        public void Copy(string bucketName, string sourceKey, string destinationKey)
        {
            try
            {
                var request = new CopyObjectRequest
                {
                    SourceBucket = bucketName,
                    SourceKey = sourceKey,
                    DestinationBucket = bucketName,
                    DestinationKey = destinationKey,
                    CannedACL = S3CannedACL.PublicRead
                };

                TransferUtility.S3Client.CopyObject(request);
            }
            catch (AmazonS3Exception e)
            {
                LogAmazonS3Exception("Copy has failed.", e);
                throw;
            }
        }

        /// <summary>
        /// Deletes the item stored under the specified bucket.
        /// </summary>
        public void Delete(string bucketName, string key)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };
                TransferUtility.S3Client.DeleteObject(request);
            }
            catch (AmazonS3Exception e)
            {
                LogAmazonS3Exception("Delete has failed.", e);
                throw;
            }
        }


        private void LogAmazonS3Exception(string message, AmazonS3Exception e)
        {
            Logger.Error("{0}\n\r{1}\n\r{2}", message, 
            String.Format("Amazon error code: {0}", string.IsNullOrEmpty(e.ErrorCode) ? "None" : e.ErrorCode),
            String.Format("Exception message: {0}", e.Message));
        }
    }
}
