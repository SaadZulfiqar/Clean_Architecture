using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Models;

namespace DataLoadTool.Core.Interfaces
{
    public interface IUserService
    {
        Task SaveUser(User user);
        Task<User> GetUserByTenantIdAndSortKey(string tenantId, string sortKey);
        Task<IEnumerable<User>> GetUsersByTenantId(string tenantId);
        Task<User> GetUserByEmail(string email);
        Task<IEnumerable<User>> GetAllUsers();
        Task<string> UpdateUser(User request);
        Task Register(Login login);
        Task<Login> Login(Login login);
    }
}
