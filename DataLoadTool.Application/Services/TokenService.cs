using DataLoadTool.Application.Utilities;
using DataLoadTool.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataLoadTool.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _key;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _key = configuration["SecretKeyForSigningTokens"];
            if (string.IsNullOrEmpty(_key))
            {
                throw new ArgumentException("TokenService: JWT secret is required.");
            }
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken(string email, IDictionary<string, string>? additionalClaims = null)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("TokenService (GenerateToken): Email must be provided.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_key);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };

            if (additionalClaims != null)
            {
                foreach (var claim in additionalClaims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = UtcDateTimeGenerator.GenerateUtcDateTime().AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogInformation("Token generated for the user Email: {Email}", email);
            return tokenHandler.WriteToken(token);
        }

        public string GetEmailFromToken()
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext?.User?.Claims == null)
            {
                _logger.LogWarning("HttpContext or User Claims are null in GetUserEmail.");
                return string.Empty;
            }

            var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email claim not found in the token.");
            }
            else
            {
                _logger.LogInformation("Email: {Email} retrieved from the claims", email);
            }

            return email;
        }
    }
}
