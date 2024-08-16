using DataLoadTool.Application.Utilities;
using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;

namespace DataLoadTool.Application.Services
{
    public class SettingService : ISettingService
    {
        private readonly IDynamoDBContextWithPrefix _context;
        public SettingService(IDynamoDBContextWithPrefix context)
        {
            _context = context;
        }

        public async Task<Setting> GetSettingByTenantIdAndSortKey(string tenantId, string sortKey)
        {
            var setting = await _context.LoadItemAsync<Setting>(tenantId, sortKey);
            return setting;
        }

        public async Task<IEnumerable<Setting>> GetSettingsByTenantId(string tenantId)
        {
            var setting = await _context.QueryAsync<Setting>(tenantId);
            return setting;
        }

        public async Task<IEnumerable<Setting>> GetAllSettings()
        {
            var settings = await _context.ScanAsync<Setting>(default);
            return settings;
        }

        public async Task SaveSetting(Setting setting)
        {
            setting.Id = UniqueGuidGenerator.GenerateUniqueGuid();
            setting.Tenant_Id = UniqueGuidGenerator.GenerateUniqueGuid();
            await _context.SaveAsync<Setting>(setting);
        }

        public async Task<string> UpdateSetting(Setting request)
        {
            var setting = await _context.LoadItemAsync<Setting>(request.Tenant_Id, request.Id);
            if (setting == null)
            {
                return "not found";
            }
            await _context.SaveAsync(request);
            return "updated";
        }

    }
}
