using Microsoft.AspNetCore.Http;

namespace DataLoadTool.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string email, IDictionary<string, string>? additionalClaims = null);
        public string GetEmailFromToken();
    }
}
