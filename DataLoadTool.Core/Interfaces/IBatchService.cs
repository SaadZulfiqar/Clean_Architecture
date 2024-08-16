using DataLoadTool.Core.Entities;

namespace DataLoadTool.Core.Interfaces
{
    public interface IBatchService
    {
        Task Save(Batch batch);
        Task<Batch> GetByTenantId(string tenantId, string sortKey);
    }
}
