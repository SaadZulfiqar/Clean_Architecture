using DataLoadTool.Core.Entities;

namespace DataLoadTool.Core.Interfaces
{
    public interface IBatchFileService
    {
        Task Save(BatchFile batchFile);
        Task<BatchFile> GetByTenantId(string tenantId, string sortKey);
    }
}
