using DataLoadTool.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DataLoadTool.Application.Services
{
    public class UploadService : IUploadService
    {
        private readonly IS3FileStorageService _s3FileStorageService;
        private readonly string _bucketName;
        public UploadService(IS3FileStorageService s3FileStorageService, IConfiguration configuration)
        {
            _s3FileStorageService = s3FileStorageService;
            _bucketName = configuration["AWS:S3:TenantDataBucketName"];
        }
        public Task EnsureFolderExistsAsync(string folderKey)
        {
            return _s3FileStorageService.EnsureFolderExistsAsync(_bucketName, folderKey);
        }
        public Task UploadToS3Async(string key, Stream fileStream)
        {
            return _s3FileStorageService.UploadFileAsync(_bucketName, key, fileStream);
        }
    }
}
