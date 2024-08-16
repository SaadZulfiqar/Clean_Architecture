namespace DataLoadTool.Core.Interfaces
{
    public interface IUploadService
    {
        Task UploadToS3Async(string filePath, Stream fileStream);
        Task EnsureFolderExistsAsync(string folderKey);
    }
}
