using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DataLoadTool.Application.Utilities;
using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;
using DataLoadTool.Core.Models;
using Microsoft.Extensions.Configuration;

namespace DataLoadTool.Infrastructure.Persistence
{
    public class DynamoDBSeedService : ISeedService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private readonly IDynamoDBContextWithPrefix _context;
        private readonly string _defaultPrefix;
        public DynamoDBSeedService(IAmazonDynamoDB dynamoDBClient, IDynamoDBContextWithPrefix context, IConfiguration configuration)
        {
            _dynamoDBClient = dynamoDBClient;
            _context = context;
            _defaultPrefix = configuration["AWS:DynamoDB:TablePrefix"];
        }
        public async Task SeedAsync()
        {
            await SeedSuperUsersAsync();
            string tenantId = UniqueGuidGenerator.GenerateUniqueGuid();
            await SeedTenantsAsync(tenantId);
            await SeedRolesAsync(tenantId);
            await SeedUsersAsync(tenantId);
            await SeedSettingAsync(tenantId);
        }
        private async Task SeedSuperUsersAsync()
        {
            var tableName = "super_user";
            if (await IsTableEmptyAsync(tableName))
            {
                HashedPasswordResult hashedResult = PasswordHasher.GenerateHashedPassword("testing");

                var superUsers = new List<SuperUser>
                {
                    new SuperUser { Id = UniqueGuidGenerator.GenerateUniqueGuid(), Email = "MarkRollins@aria.com", Hash = hashedResult.Hash,  Salt = hashedResult.Salt, Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() },
                    new SuperUser { Id = UniqueGuidGenerator.GenerateUniqueGuid(), Email = "SaadZulfiqar@aria.com", Hash = hashedResult.Hash,  Salt = hashedResult.Salt, Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() }
                };

                foreach (var superUser in superUsers)
                {
                    await _context.SaveAsync(superUser);
                }
            }
        }
        private async Task SeedTenantsAsync(string tenantId)
        {
            var tableName = "tenant";
            if (await IsTableEmptyAsync(tableName))
            {
                var tenants = new List<Tenant>
                {
                    new Tenant { Id = tenantId, Name = "Verisure", Description = "Verisure Spain", Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() },
                    new Tenant { Id = UniqueGuidGenerator.GenerateUniqueGuid(), Name = "JFM", Description = "JFM", Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() },
                    new Tenant { Id = UniqueGuidGenerator.GenerateUniqueGuid(), Name = "BEM", Description = "BEM", Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() }

                };

                foreach (var tenant in tenants)
                {
                    await _context.SaveAsync(tenant);
                }
            }
        }
        private async Task SeedRolesAsync(string tenantId)
        {
            var tableName = "role";
            if (await IsTableEmptyAsync(tableName))
            {
                var roles = new List<Role>
                {
                    new Role { Tenant_Id = tenantId, Id = UniqueGuidGenerator.GenerateUniqueGuid(), Name = "Customer Data", Description = "Customer Data", Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() },
                    new Role { Tenant_Id = tenantId, Id = UniqueGuidGenerator.GenerateUniqueGuid(), Name = "Product Data", Description = "Product Data", Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() }
                };

                foreach (var role in roles)
                {
                    await _context.SaveAsync(role);
                }
            }
        }
        private async Task SeedUsersAsync(string tenantId)
        {
            var userTable = "user";
            if (await IsTableEmptyAsync(userTable))
            {
                HashedPasswordResult hashedResult = PasswordHasher.GenerateHashedPassword("testing");

                var users = new List<User>
                {
                    new User { Tenant_Id = tenantId, Id = UniqueGuidGenerator.GenerateUniqueGuid(), Email = "Aizaz@aria.com", Hash = hashedResult.Hash,  Salt = hashedResult.Salt, Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() },
                    new User { Tenant_Id = tenantId, Id = UniqueGuidGenerator.GenerateUniqueGuid(), Email = "MarkRollins@aria.com", Hash = hashedResult.Hash,  Salt = hashedResult.Salt, Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() },
                    new User { Tenant_Id = tenantId, Id = UniqueGuidGenerator.GenerateUniqueGuid(), Email = "SaadZulfiqar@aria.com", Hash = hashedResult.Hash,  Salt = hashedResult.Salt, Status = true, CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() }
                };

                foreach (var user in users)
                {
                    await _context.SaveAsync(user);
                }
            }
        }
        private async Task SeedSettingAsync(string tenantId)
        {
            var settingTable = "setting";
            if (await IsTableEmptyAsync(settingTable))
            {
                var setting = new List<Setting>
                {
                    new Setting { Tenant_Id = tenantId, Id = UniqueGuidGenerator.GenerateUniqueGuid(), Configuration= new List<string> { "test1", "test2", "test3" } , CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() },
                    new Setting { Tenant_Id = tenantId, Id = UniqueGuidGenerator.GenerateUniqueGuid(), Configuration= new List<string> { "test4", "test5", "test6" } , CreatedBy="Seed", CreatedDate=UtcDateTimeGenerator.GenerateUtcDateTime() },
                };

                foreach (var set in setting)
                {
                    await _context.SaveAsync(set);
                }
            }
        }
        private async Task<bool> IsTableEmptyAsync(string tableName)
        {
            var scanRequest = new ScanRequest
            {
                TableName = _defaultPrefix + tableName,
                Limit = 1
            };
            var response = await _dynamoDBClient.ScanAsync(scanRequest);
            return response.Count == 0;
        }
    }
}
