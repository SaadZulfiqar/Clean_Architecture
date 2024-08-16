namespace DataLoadTool.Core.Interfaces
{
    public interface IS3FileStorageService
    {
        Task UploadFileAsync(string bucketName, string key, Stream inputStream);
        Task EnsureFolderExistsAsync(string bucketName, string folderKey);
        Task<MemoryStream> DownloadFileAsync(string bucketName, string key);
    }
}
