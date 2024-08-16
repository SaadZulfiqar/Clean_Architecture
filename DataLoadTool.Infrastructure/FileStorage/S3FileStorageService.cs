using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DataLoadTool.Core.Interfaces;

namespace DataLoadTool.Infrastructure.FileStorage
{
    public class S3FileStorageService : IS3FileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        public S3FileStorageService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
        public async Task UploadFileAsync(string bucketName, string key, Stream inputStream)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = inputStream,
                Key = key,
                BucketName = bucketName,
                CannedACL = S3CannedACL.Private
            };

            await fileTransferUtility.UploadAsync(uploadRequest);
        }
        public async Task<MemoryStream> DownloadFileAsync(string bucketName, string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key,
            };
            using GetObjectResponse response = await _s3Client.GetObjectAsync(request);
            using Stream responseStream = response.ResponseStream;

            var memoryStream = new MemoryStream();
            await responseStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
        public async Task EnsureFolderExistsAsync(string bucketName, string folderKey)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = folderKey,
                MaxKeys = 1
            };

            var response = await _s3Client.ListObjectsV2Async(request);
            if (response.S3Objects.Count == 0)
            {
                // Create a placeholder object to represent the folder
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = folderKey,
                    ContentBody = string.Empty
                };

                await _s3Client.PutObjectAsync(putRequest);
            }
        }
    }
}
