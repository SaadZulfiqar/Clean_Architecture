using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;

namespace DataLoadTool.Application.Services
{
    public class BatchFileService : IBatchFileService
    {
        private readonly IDynamoDBContextWithPrefix _context;
        public BatchFileService(IDynamoDBContextWithPrefix context)
        {
            _context = context;
        }

        public async Task Save(BatchFile batchFile)
        {
            await _context.SaveAsync(batchFile);
        }
        public async Task<BatchFile> GetByTenantId(string tenantId, string sortKey)
        {
            var batchFile = await _context.LoadItemAsync<BatchFile>(tenantId, sortKey);
            return batchFile;
        }
    }
}
