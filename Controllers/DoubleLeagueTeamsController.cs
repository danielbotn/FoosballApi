using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.DoubleLeagueTeams;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoubleLeagueTeamsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDoubleLeagueTeamService _doubleLeagueTeamService;

        public DoubleLeagueTeamsController(IMapper mapper, IDoubleLeagueTeamService doubleLeagueTeamService)
        {
            _mapper = mapper;
            _doubleLeagueTeamService = doubleLeagueTeamService;
        }

        [HttpGet("{leagueId}")]
        public ActionResult<IEnumerable<DoubleLeagueTeamReadDto>> GetDoubleLeagueTeamsByLeagueId(int leagueId)
        {
            try
            {
                string userId = User.Identity.Name;

                bool permission = _doubleLeagueTeamService.CheckLeaguePermission(leagueId, int.Parse(userId));

                if (!permission)
                    return Forbid();

                var allTeams = _doubleLeagueTeamService.GetDoubleLeagueTeamsByLeagueId(leagueId);

                if (allTeams == null)
                    return NotFound();

                return Ok(_mapper.Map<IEnumerable<DoubleLeagueTeamReadDto>>(allTeams));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}