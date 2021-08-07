using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FoosballApi.Dtos.DoubleLeaguePlayers;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoubleLeaguePlayersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDoubleLeaguePlayerService _doubleLeaugePlayerService;
        private readonly IDoubleLeagueTeamService _doubleLeagueTeamService;

        public DoubleLeaguePlayersController(IMapper mapper, IDoubleLeaguePlayerService doubleLeaugePlayerService, IDoubleLeagueTeamService doubleLeagueTeamService)
        {
            _mapper = mapper;
            _doubleLeaugePlayerService = doubleLeaugePlayerService;
            _doubleLeagueTeamService = doubleLeagueTeamService;
        }

        [HttpGet("{leagueId}")]
        public async Task<ActionResult> GetDoubleLeaguePlayersyLeagueId(int leagueId)
        {
            try
            {
                string userId = User.Identity.Name;

                bool permission = _doubleLeagueTeamService.CheckLeaguePermission(leagueId, int.Parse(userId));

                if (!permission)
                    return Forbid();

                var allPlayers = await _doubleLeaugePlayerService.GetDoubleLeaguePlayersyLeagueId(leagueId);

                if (allPlayers == null)
                    return NotFound();

                return Ok(_mapper.Map<IEnumerable<DoubleLeaguePlayerReadDto>>(allPlayers));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}