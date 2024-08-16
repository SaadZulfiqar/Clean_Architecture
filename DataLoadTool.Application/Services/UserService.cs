using DataLoadTool.Application.Utilities;
using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;
using DataLoadTool.Core.Models;
using Microsoft.Extensions.Logging;

namespace DataLoadTool.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDynamoDBContextWithPrefix _context;
        private readonly ILogger<UserService> _logger;
        private readonly ITokenService _tokenService;
        private readonly string gsi_email = "_email";
        public UserService(IDynamoDBContextWithPrefix context, ILogger<UserService> logger, ITokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
        }
        public async Task<User> GetUserByTenantIdAndSortKey(string tenantId, string sortKey)
        {
            var user = await _context.LoadItemAsync<User>(tenantId, sortKey);
            return user;
        }
        public async Task<IEnumerable<User>> GetUsersByTenantId(string tenantId)
        {
            var user = await _context.QueryAsync<User>(tenantId);
            return user;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var users = await _context.QueryAsyncByIndex<User>(email, indexName: gsi_email);

            if (users != null && users.Count() > 1)
            {
                throw new Exception($"There are more users with email: {email}, which should not be the case.");
            }

            return users?.FirstOrDefault();
        }
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _context.ScanAsync<User>(default);
            return users;
        }
        public async Task SaveUser(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            user.Tenant_Id = Guid.NewGuid().ToString();
            await _context.SaveAsync<User>(user);
        }
        public async Task<string> UpdateUser(User request)
        {
            var user = await _context.LoadItemAsync<User>(request.Tenant_Id, request.Id);
            if (user == null)
            {
                return "not found";
            }
            await _context.SaveAsync(request);
            return "updated";
        }
        public async Task<Login> Login(Login login)
        {
            _logger.LogInformation("Login the user.");

            Common.ValidateLoginArguments(login);

            var existingUser = await GetUserByEmail(login.Email);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User does not exist with this email.");
            }

            if (!PasswordHasher.VerifyPassword(login.Password, existingUser.Hash, existingUser.Salt))
            {
                throw new ArgumentException("Invalid password.");
            }

            login.Token = _tokenService.GenerateToken(existingUser.Email);
            login.Password = null;
            login.IsSuperUser = false;
            login.Tenant_id = existingUser.Tenant_Id;

            return login;
        }
        public async Task Register(Login login)
        {
            _logger.LogInformation("Creating the user.");

            Common.ValidateLoginArguments(login, false);

            var existingUser = await GetUserByEmail(login.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists with this email.");
            }

            var hashedResult = PasswordHasher.GenerateHashedPassword(login.Password);

            var user = new User
            {
                Id = UniqueGuidGenerator.GenerateUniqueGuid(),
                Tenant_Id = login.Tenant_id,
                CreatedBy = _tokenService.GetEmailFromToken(),
                CreatedDate = UtcDateTimeGenerator.GenerateUtcDateTime(),
                Email = login.Email,
                Hash = hashedResult.Hash,
                Salt = hashedResult.Salt,
                Status = true
            };

            await _context.SaveAsync(user);
        }
    }
}
