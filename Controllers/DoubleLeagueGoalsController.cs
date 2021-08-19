using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FoosballApi.Dtos.DoubleLeagueGoals;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoubleLeagueGoalsController : ControllerBase
    {
        private readonly IDoubleLeagueGoalService _goalService;
        private readonly IDoubleLeaugeMatchService _doubleLeaugeMatchService;
        private readonly IMapper _mapper;

        public DoubleLeagueGoalsController(IDoubleLeagueGoalService goalService, IDoubleLeaugeMatchService doubleLeaugeMatchService, IMapper mapper)
        {
            _goalService = goalService;
            _doubleLeaugeMatchService = doubleLeaugeMatchService;
            _mapper = mapper;
        }

        [HttpGet("")]
        public async Task<ActionResult> GetAllDoubleLeaguesMatchesByLeagueId(int matchId)
        {
            try
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

                bool hasPermission = _doubleLeaugeMatchService.CheckMatchAccess(matchId, int.Parse(userId), int.Parse(currentOrganisationId));

                if (!hasPermission)
                    return Forbid();

                var allGoals = await _goalService.GetAllDoubleLeagueGoalsByMatchId(matchId);

                return Ok(_mapper.Map<IEnumerable<DoubleLeagueGoalReadDto>>(allGoals));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{goalId}")]
        public async Task<ActionResult> GetDoubleLeagueMatchById(int goalId)
        {
            try 
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

                bool permission = _goalService.CheckPermissionByGoalId(goalId, int.Parse(userId));

                if (!permission)
                    return Forbid();
                
                var goaldData = await _goalService.GetDoubleLeagueGoalById(goalId);

                return Ok(_mapper.Map<DoubleLeagueGoalReadDto>(goaldData));
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}