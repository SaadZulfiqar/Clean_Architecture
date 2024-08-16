using DataLoadTool.Core.DTOs;
using DataLoadTool.Core.Entities;

namespace DataLoadTool.Core.Interfaces
{
    public interface ITenantService
    {
        Task Create(TenantDTO tenantDTO);
        Task Update(TenantDTO tenantDTO);
        Task<IEnumerable<Tenant>> GetAll();
        Task<Tenant> GetById(string id);
        Task Delete(string id);
    }
}
