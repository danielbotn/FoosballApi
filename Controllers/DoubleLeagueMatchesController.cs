using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.DoubleLeagueMatches;
using FoosballApi.Models.DoubleLeagueMatches;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoubleLeagueMatchesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDoubleLeaugeMatchService _doubleLeaugeMatchService;

        public DoubleLeagueMatchesController(IMapper mapper, IDoubleLeaugeMatchService doubleLeaugeMatchService)
        {
            _mapper = mapper;
            _doubleLeaugeMatchService = doubleLeaugeMatchService;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<AllMatchesModelReadDto>> GetAllDoubleLeaguesMatchesByLeagueId(int leagueId)
        {
            try
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

                bool permission = _doubleLeaugeMatchService.CheckLeaguePermission(leagueId, int.Parse(userId));

                if (!permission)
                    return Forbid();

                var allMatches = _doubleLeaugeMatchService.GetAllMatchesByOrganisationId(int.Parse(currentOrganisationId), leagueId);

                return Ok(_mapper.Map<IEnumerable<AllMatchesModelReadDto>>(allMatches));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("matchId")]
        public ActionResult<AllMatchesModelReadDto> GetDoubleLeagueMatchById(int matchId)
        {
            try 
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

                bool permission = _doubleLeaugeMatchService.CheckMatchAccess(matchId, int.Parse(userId), int.Parse(currentOrganisationId));

                if (!permission)
                    return Forbid();
                
                var matchData = _doubleLeaugeMatchService.GetDoubleLeagueMatchById(matchId);

                return Ok(_mapper.Map<AllMatchesModelReadDto>(matchData));
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
    }
}