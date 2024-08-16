using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;

namespace DataLoadTool.Application.Services
{
    public class BatchService : IBatchService
    {
        private readonly IDynamoDBContextWithPrefix _context;
        public BatchService(IDynamoDBContextWithPrefix context)
        {
            _context = context;
        }
        public async Task Save(Batch batch)
        {
            await _context.SaveAsync(batch);
        }
        public async Task<Batch> GetByTenantId(string tenantId, string sortKey)
        {
            var batch = await _context.LoadItemAsync<Batch>(tenantId, sortKey);
            return batch;
        }
    }
}
