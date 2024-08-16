using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataLoadTool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("GetUserByTenantIdAndSortKey")]
        public async Task<ActionResult> GetUserByTenantIdAndSortKey(string tenantId, string sortKey)
        {
            try
            {
                var user = await _userService.GetUserByTenantIdAndSortKey(tenantId, sortKey);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("GetUserByTenantId")]
        public async Task<ActionResult> GetUsersByTenantId(string tenantId)
        {
            try
            {

                var users = await _userService.GetUsersByTenantId(tenantId);
                if (users == null)
                {
                    return NotFound();
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddUser(User user)
        {
            try
            {
                await _userService.SaveUser(user);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost]
        [Route("UpdateUser")]
        public async Task<ActionResult> UpdateUser(User request)
        {
            try
            {
                var res = await _userService.UpdateUser(request);
                if (!string.IsNullOrEmpty(res) && res == "not found")
                {
                    return NotFound();
                }
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
