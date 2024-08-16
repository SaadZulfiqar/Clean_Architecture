using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Models;

namespace DataLoadTool.Core.Interfaces
{
    public interface ISuperUserService
    {
        Task SaveSuperUser(SuperUser superUser);
        Task<IEnumerable<SuperUser>> GetAllSuperUsers();
        Task<IEnumerable<SuperUser>> GetSuperUserById(string id);
        Task<string> UpdateSuperUser(SuperUser request);
        Task Register(Login login);
        Task<Login> Login(Login login);
    }
}
