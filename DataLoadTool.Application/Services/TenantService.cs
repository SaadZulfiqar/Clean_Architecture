using DataLoadTool.Application.Utilities;
using DataLoadTool.Core.DTOs;
using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataLoadTool.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IDynamoDBContextWithPrefix _context;
        private readonly ILogger<TenantService> _logger;
        private readonly ITokenService _tokenService;

        public TenantService(IDynamoDBContextWithPrefix context, ILogger<TenantService> logger, ITokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
        }
        private async Task<bool> IsUniqueTenant(TenantDTO tenant, bool isUpdate = false)
        {
            if (string.IsNullOrEmpty(tenant.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            var tenants = await GetAll();

            if (isUpdate)
            {
                // Remove the current tenant from the list when checking for uniqueness during an update
                tenants = tenants.Where(t => t.Id != tenant.Id);
            }

            // Check for duplicate tenant names, ignoring case and trimming spaces
            if (tenants.Any(t => string.Equals(t.Name.Trim(), tenant.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return true;
        }
        public async Task<IEnumerable<Tenant>> GetAll()
        {
            _logger.LogInformation("Fetching all tenants.");
            return await _context.ScanAsync<Tenant>(default);
        }
        public async Task Create(TenantDTO tenantDTO)
        {
            _logger.LogInformation("Creating a new tenant.");

            if (string.IsNullOrWhiteSpace(tenantDTO.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            var isUniqueTenant = await IsUniqueTenant(tenantDTO);
            if (!isUniqueTenant)
            {
                throw new Exception("Tenant with the same name already exists.");
            }

            Tenant tenant = new Tenant
            {
                Id = UniqueGuidGenerator.GenerateUniqueGuid(),
                Name = tenantDTO.Name,
                Description = tenantDTO.Description,
                Status = tenantDTO.Status,
                CreatedBy = _tokenService.GetEmailFromToken(),
                CreatedDate = UtcDateTimeGenerator.GenerateUtcDateTime()
            };

            await _context.SaveAsync(tenant);
            _logger.LogInformation($"Tenant created with ID: {tenant.Id}");
        }
        public async Task Update(TenantDTO tenantDTO)
        {
            _logger.LogInformation("Updating tenant.");

            if (string.IsNullOrWhiteSpace(tenantDTO.Id))
            {
                throw new ArgumentException("Tenant ID is required.");
            }

            if (string.IsNullOrWhiteSpace(tenantDTO.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            // Perform uniqueness check for updates
            var isUniqueTenant = await IsUniqueTenant(tenantDTO, isUpdate: true);
            if (!isUniqueTenant)
            {
                throw new Exception("Tenant with the same name or domains already exists.");
            }

            Tenant tenant = await GetById(tenantDTO.Id);
            if (tenant == null)
            {
                throw new Exception($"Tenant with ID: {tenantDTO.Id} does not exist.");
            }

            tenant.Name = tenantDTO.Name;
            tenant.Description = tenantDTO.Description;
            tenant.Status = tenantDTO.Status;
            tenant.UpdatedBy = _tokenService.GetEmailFromToken();
            tenant.UpdatedDate = UtcDateTimeGenerator.GenerateUtcDateTime();

            await _context.SaveAsync(tenant);
            _logger.LogInformation($"Tenant updated with ID: {tenant.Id}");
        }
        public async Task<Tenant> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Tenant ID is required.");
            }

            _logger.LogInformation($"Fetching tenant by ID: {id}");

            var tenant = await _context.LoadItemAsync<Tenant>(id);
            if (tenant == null)
            {
                _logger.LogWarning($"Tenant with ID {id} not found.");
            }
            return tenant;
        }
        public async Task Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Tenant ID is required.");
            }
            _logger.LogInformation($"Deleting tenant by ID: {id}");
            await _context.DeleteAsync<Tenant>(id);
        }
    }
}
