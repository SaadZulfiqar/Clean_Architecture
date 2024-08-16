using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DataLoadTool.Application.Utilities;
using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;
using DataLoadTool.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;
using System.Xml.Linq;

namespace DataLoadTool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : Controller
    {
        //private readonly IDynamoDBContextWithPrefix _context;
        private readonly ISettingService _settingService;
        public SettingController(IDynamoDBContextWithPrefix context, ISettingService settingService)
        {
            //_context = context;
            _settingService = settingService;
        }

        [HttpGet]
        [Route("GetSettingByTenantIdAndSortKey")]
        public async Task<ActionResult> GetSettingByTenantIdAndSortKey(string tenantId, string sortKey)
        {
            try
            {
                var setting = await _settingService.GetSettingByTenantIdAndSortKey(tenantId, sortKey);
                if (setting == null)
                {
                    return NotFound();
                }
                return Ok(setting);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("GetSettingByTenantId")]
        public async Task<ActionResult> GetSettingsByTenantId(string tenantId)
        {
            try
            {

                var settings = await _settingService.GetSettingsByTenantId(tenantId);
                if (settings == null)
                {
                    return NotFound();
                }
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddSetting(Setting setting)
        {
            try
            {
                await _settingService.SaveSetting(setting);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost]
        [Route("UpdateSetting")]
        public async Task<ActionResult> UpdateSetting(Setting request)
        {
            try
            {
                var res = await _settingService.UpdateSetting(request);
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
        [Route("GetAllSettings")]
        public async Task<ActionResult> GetAllSettings()
        {
            try
            {
                var settings = await _settingService.GetAllSettings();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
