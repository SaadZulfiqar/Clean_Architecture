using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataLoadTool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperUserController : Controller
    {
        private readonly ISuperUserService _superUserService;
        public SuperUserController(ISuperUserService superUserService)
        {
            _superUserService = superUserService;
        }

        [HttpPost]
        public async Task<ActionResult> AddSuperUser(SuperUser superUser)
        {
            try
            {
                await _superUserService.SaveSuperUser(superUser);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet]
        [Route("GetSuperUserById")]
        public async Task<ActionResult> GetSuperUserById(string id)
        {
            try
            {

                var superUser = await _superUserService.GetSuperUserById(id);
                if (superUser == null)
                {
                    return NotFound();
                }
                return Ok(superUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("UpdateSuperUser")]
        public async Task<ActionResult> UpdateSuperUser(SuperUser request)
        {
            try
            {
                var res = await _superUserService.UpdateSuperUser(request);
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
        [Route("GetAllSuperUsers")]
        public async Task<ActionResult> GetAllSuperUsers()
        {
            try
            {
                var superUsers = await _superUserService.GetAllSuperUsers();
                return Ok(superUsers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
