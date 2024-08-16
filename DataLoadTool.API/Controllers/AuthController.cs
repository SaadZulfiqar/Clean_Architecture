using DataLoadTool.Core.Interfaces;
using DataLoadTool.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ISuperUserService _superUserService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ISuperUserService superUserService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _superUserService = superUserService;
        _logger = logger;
    }

    [HttpPost("login/tenant")]
    public async Task<IActionResult> TenantLogin([FromBody] Login login)
    {
        try
        {
            var result = await _userService.Login(login);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, $"TenantLogin failed: {ex.Message}");
        }
    }

    [HttpPost("login/superuser")]
    public async Task<IActionResult> SuperUserLogin([FromBody] Login login)
    {
        try
        {
            var result = await _superUserService.Login(login);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, $"SuperUserLogin failed: {ex.Message}");
        }
    }

    [HttpPost("register/tenant")]
    [Authorize]
    public async Task<IActionResult> TenantUserRegister([FromBody] Login login)
    {
        try
        {
            await _userService.Register(login);
            return Ok(new { Message = "User created successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, $"TenantRegister failed: {ex.Message}");
        }
    }

    [HttpPost("register/superuser")]
    [Authorize]
    public async Task<IActionResult> SuperUserRegisterAsync([FromBody] Login login)
    {
        try
        {
            await _superUserService.Register(login);
            return Ok(new { Message = "Super user created successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, $"SuperUserRegister failed: {ex.Message}");
        }
    }
}
