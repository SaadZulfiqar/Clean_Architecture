using DataLoadTool.Core.Entities;

namespace DataLoadTool.Core.Interfaces
{
    public interface ISettingService
    {
        Task SaveSetting(Setting setting);
        Task<Setting> GetSettingByTenantIdAndSortKey(string tenantId, string sortKey);
        Task<IEnumerable<Setting>> GetSettingsByTenantId(string tenantId);
        Task<IEnumerable<Setting>> GetAllSettings();
        Task<string> UpdateSetting(Setting request);
    }
}
