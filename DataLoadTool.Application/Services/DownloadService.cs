using DataLoadTool.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DataLoadTool.Application.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly IS3FileStorageService _s3FileStorageService;
        private readonly string _bucketName;
        public DownloadService(IS3FileStorageService s3FileStorageService, IConfiguration configuration)
        {
            _s3FileStorageService = s3FileStorageService;
            _bucketName = configuration["AWS:S3:TenantDataBucketName"];
        }
        public Task<MemoryStream> DownloadFromS3Async(string filePath)
        {
            return _s3FileStorageService.DownloadFileAsync(_bucketName, filePath);
        }
    }
}
