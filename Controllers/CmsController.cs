using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FoosballApi.Dtos.DoubleMatches;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CmsController : ControllerBase
    {
        private readonly ICmsService _cmsService;

        public CmsController(ICmsService cmsService)
        {
            _cmsService = cmsService;
        }

        [HttpPost("hardcoded-strings")]
        public async Task<IActionResult> GetHardcodedStrings(string language)
        {
            try 
            {
                var data = await _cmsService.GetHardcodedStrings(language);

                return Ok(data);
            }
            catch(Exception e)
            {
                return UnprocessableEntity(e);
            }
        }
    }
}