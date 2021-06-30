using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.DoubleGoals;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FreehandDoubleGoalsController : ControllerBase
    {
        private readonly IFreehandDoubleGoalService _doubleFreehandGoalervice;

        private readonly IFreehandDoubleMatchService _doubleFreehandMatchService;
        private readonly IMapper _mapper;

        public FreehandDoubleGoalsController(IFreehandDoubleGoalService doubleFreehandGoalervice, IMapper mapper, IFreehandDoubleMatchService doubleFreehandMatchService)
        {
            _mapper = mapper;
            _doubleFreehandGoalervice = doubleFreehandGoalervice;
            _doubleFreehandMatchService = doubleFreehandMatchService;
        }

        [HttpGet("goals/{matchId}")]
        public ActionResult<IEnumerable<FreehandDoubleGoalsJoinDto>> GetFreehandDoubleGoalsByMatchId()
        {
            string matchId = RouteData.Values["matchId"].ToString();
            string userId = User.Identity.Name;

            bool access = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), int.Parse(matchId));

            if (!access)
                return Forbid();

            var allGoals = _doubleFreehandGoalervice.GetAllFreehandGoals(int.Parse(matchId), int.Parse(userId));

            if (allGoals == null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<FreehandDoubleGoalsJoinDto>>(allGoals));

        }

        [HttpGet("{goalId}")]
        public ActionResult<FreehandDoubleGoalReadDto> GetFreehandDoubleGoalById(int goalId, int matchId)
        {
            string userId = User.Identity.Name;

            bool matchAccess = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), matchId);

            if (!matchAccess)
                return Forbid();

            bool goalAccess = _doubleFreehandGoalervice.CheckGoalPermission(int.Parse(userId), matchId, goalId);

            if (!goalAccess)
                return Forbid();

            var freehandDoubleGoal = _doubleFreehandGoalervice.GetFreehandDoubleGoal(goalId);

            return Ok(_mapper.Map<FreehandDoubleGoalReadDto>(freehandDoubleGoal));
        }

    }
}