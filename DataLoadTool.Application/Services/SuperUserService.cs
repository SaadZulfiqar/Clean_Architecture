using DataLoadTool.Application.Utilities;
using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;
using DataLoadTool.Core.Models;
using Microsoft.Extensions.Logging;

namespace DataLoadTool.Application.Services
{
    public class SuperUserService : ISuperUserService
    {
        private readonly IDynamoDBContextWithPrefix _context;
        private readonly ILogger<SuperUserService> _logger;
        private readonly ITokenService _tokenService;
        public SuperUserService(IDynamoDBContextWithPrefix context, ILogger<SuperUserService> logger, ITokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
        }

        private async Task<SuperUser?> GetSuperUserByEmailAsync(string email)
        {
            var users = await GetAllSuperUsers();
            return users?.FirstOrDefault(superUser => superUser.Email.Trim().ToLower() == email.Trim().ToLower());
        }

        public async Task<IEnumerable<SuperUser>> GetAllSuperUsers()
        {
            var superUsers = await _context.ScanAsync<SuperUser>(default);
            return superUsers;
        }

        public async Task<IEnumerable<SuperUser>> GetSuperUserById(string id)
        {
            var superUser = await _context.QueryAsync<SuperUser>(id);
            return superUser;
        }

        public async Task SaveSuperUser(SuperUser superUser)
        {
            superUser.Id = UniqueGuidGenerator.GenerateUniqueGuid();
            await _context.SaveAsync<SuperUser>(superUser);
        }
        public async Task<string> UpdateSuperUser(SuperUser request)
        {
            var superUser = await _context.LoadItemAsync<SuperUser>(request.Id, request.Email);
            if (superUser == null)
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

            var existingUser = await GetSuperUserByEmailAsync(login.Email);
            if (existingUser == null)
            {
                throw new InvalidOperationException("Super user does not exist with this email.");
            }

            if (!PasswordHasher.VerifyPassword(login.Password, existingUser.Hash, existingUser.Salt))
            {
                throw new ArgumentException("Invalid password.");
            }

            login.Token = _tokenService.GenerateToken(existingUser.Email);
            login.Password = null;
            login.IsSuperUser = true;

            return login;
        }
        public async Task Register(Login login)
        {
            _logger.LogInformation("Creating the user.");

            Common.ValidateLoginArguments(login, true);

            var existingUser = await GetSuperUserByEmailAsync(login.Email);
            if (existingUser != null)
            {
                throw new Exception("Super user already exists with this email.");
            }

            var hashedResult = PasswordHasher.GenerateHashedPassword(login.Password);

            var superUser = new SuperUser
            {
                Id = UniqueGuidGenerator.GenerateUniqueGuid(),
                CreatedBy = _tokenService.GetEmailFromToken(),
                CreatedDate = UtcDateTimeGenerator.GenerateUtcDateTime(),
                Email = login.Email,
                Hash = hashedResult.Hash,
                Salt = hashedResult.Salt,
                Status = true
            };

            await SaveSuperUser(superUser);
        }
    }
}
