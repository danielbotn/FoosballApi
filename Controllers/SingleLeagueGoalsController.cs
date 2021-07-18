using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.SingleLeagueGoals;
using FoosballApi.Models.SingleLeagueGoals;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SingleLeagueGoalsController : ControllerBase
    {
        private readonly ISingleLeagueGoalService _singleLeagueGoalService;
        private readonly ISingleLeagueMatchService _singleLeagueMatchService;
        private readonly IMapper _mapper;

        public SingleLeagueGoalsController(ISingleLeagueGoalService singleLeagueGoalService, ISingleLeagueMatchService singleLeagueMatchService, IMapper mapper)
        {
            _mapper = mapper;
            _singleLeagueGoalService = singleLeagueGoalService;
            _singleLeagueMatchService = singleLeagueMatchService;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<SingleLeagueGoalReadDto>> GetAllSingleLeagueGoalsByMatchId(int leagueId, int matchId)
        {
            string userId = User.Identity.Name;

            bool permission = _singleLeagueMatchService.CheckLeaguePermission(leagueId, int.Parse(userId));

            if (!permission)
                return Forbid();

            var allGoals = _singleLeagueGoalService.GetAllSingleLeagueGoalsByMatchId(matchId);

            return Ok(_mapper.Map<IEnumerable<SingleLeagueGoalReadDto>>(allGoals));
        }

        [HttpGet("{goalId}", Name="getSingleLeagueById")]
        public ActionResult<SingleLeagueGoalReadDto> GetSingleLeagueGoalById(int goalId)
        {
            try {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

                bool permission = _singleLeagueGoalService.CheckSingleLeagueGoalPermission(int.Parse(userId), goalId, int.Parse(currentOrganisationId));

                if (!permission)
                    return Forbid();

                var goal = _singleLeagueGoalService.GetSingleLeagueGoalById(goalId);

                return Ok(_mapper.Map<SingleLeagueGoalReadDto>(goal));
            }
            catch(Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        [HttpDelete()]
        public ActionResult DeleteSingleLeagueGoalById(int goalId)
        {
            string userId = User.Identity.Name;
            string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;
            var goalItem = _singleLeagueGoalService.GetSingleLeagueGoalById(goalId);
            if (goalItem == null)
                return NotFound();

            bool hasPermission = _singleLeagueGoalService.CheckSingleLeagueGoalPermission(int.Parse(userId), goalId, int.Parse(currentOrganisationId));

            if (!hasPermission)
                return Forbid();

            _singleLeagueGoalService.DeleteSingleLeagueGoal(goalItem);

            return NoContent();
        }

        [HttpPost("")]
        public ActionResult CreateSingleLeagueGoal([FromBody] SingleLeagueCreateModel singleLeagueCreateModel)
        {
            string userId = User.Identity.Name;

            var newGoal = _singleLeagueGoalService.CreateSingleLeagueGoal(singleLeagueCreateModel);

            var singleLeagueGoalReadDto = _mapper.Map<SingleLeagueGoalReadDto>(singleLeagueCreateModel);

            return CreatedAtRoute("getSingleLeagueById", new { goalId = newGoal.Id }, singleLeagueGoalReadDto);
        }
    }
}