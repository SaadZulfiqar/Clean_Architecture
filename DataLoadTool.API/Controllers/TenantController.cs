using DataLoadTool.Core.DTOs;
using DataLoadTool.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataLoadTool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantController> _logger;
        public TenantController(ITenantService tenantService, ILogger<TenantController> logger)
        {
            _tenantService = tenantService;
            _logger = logger;
        }

        // GET: api/<TenantController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var tenants = await _tenantService.GetAll();
                return Ok(tenants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Tenant get: {ex.Message}");
            }
        }

        // GET api/<TenantController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var tenant = await _tenantService.GetById(id);
                return Ok(tenant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Tenant post: {ex.Message}");
            }
        }

        // POST api/<TenantController>
        [HttpPost]
        public async Task<IActionResult> Post(TenantDTO tenantDTO)
        {
            try
            {
                await _tenantService.Create(tenantDTO);
                return Ok(new { Message = "Tenant created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Tenant post: {ex.Message}");
            }
        }

        // PUT api/<TenantController>
        [HttpPut]
        public async Task<IActionResult> Put(TenantDTO tenantDTO)
        {
            try
            {
                await _tenantService.Update(tenantDTO);
                return Ok(new { Message = "Tenant updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Tenant put: {ex.Message}");
            }
        }

        // DELETE api/<TenantController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _tenantService.Delete(id);
                return Ok(new { Message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Tenant delete: {ex.Message}");
            }
        }
    }
}
