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

        [HttpGet("{goalId}", Name = "GetFreehandDoubleGoalById")]
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

        [HttpPost("")]
        public ActionResult CreateFreehandDoubleGoal([FromBody] FreehandDoubleGoalCreateDto freehandGoalCreateDto)
        {
            int matchId = freehandGoalCreateDto.DoubleMatchId;
            string userId = User.Identity.Name;

            bool matchAccess = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), matchId);

            if (!matchAccess)
                return Forbid();

            var newGoal = _doubleFreehandGoalervice.CreateDoubleFreehandGoal(int.Parse(userId), freehandGoalCreateDto);

            var freehandGoalReadDto = _mapper.Map<FreehandDoubleGoalReadDto>(newGoal);

            return CreatedAtRoute("GetFreehandDoubleGoalById", new { goalId = newGoal.Id }, freehandGoalReadDto);
        }

        [HttpDelete("{goalId}")]
        public ActionResult DeleteDoubleFreehandGoal(int goalId, int matchId)
        {
            string userId = User.Identity.Name;
            var goalItem = _doubleFreehandGoalervice.GetFreehandDoubleGoal(goalId);
            if (goalItem == null)
                return NotFound();

            bool hasPermission = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), matchId);

            if (!hasPermission)
                return Forbid();

            _doubleFreehandGoalervice.DeleteFreehandGoal(goalItem);

            return NoContent();
        }
    }
}