namespace DataLoadTool.Core.Interfaces
{
    public interface IDownloadService
    {
        Task<MemoryStream> DownloadFromS3Async(string folderKey);
    }
}
