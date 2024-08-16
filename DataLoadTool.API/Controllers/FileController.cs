using DataLoadTool.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataLoadTool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IUploadCustomerDataUseCase _uploadCustomerDataUseCase;
        private readonly ILogger<FileController> _logger;
        private readonly ITokenService _tokenService;

        public FileController(IUploadCustomerDataUseCase uploadCustomerDataUseCase, ITokenService tokenService, ILogger<FileController> logger)
        {
            _uploadCustomerDataUseCase = uploadCustomerDataUseCase;
            _tokenService = tokenService;
            _logger = logger;
        }

        // GET: api/<FileController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<FileController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FileController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files, [FromForm] string tenant_id)
        {
            try
            {
                await _uploadCustomerDataUseCase.Upload(files, tenant_id);
                return Ok(new { Message = "File (s) uploaded successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Uploading failed: {ex.Message}");
            }
        }



        // PUT api/<FileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
