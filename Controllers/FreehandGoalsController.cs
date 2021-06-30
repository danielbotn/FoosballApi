using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Goals;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FreehandGoalsController : ControllerBase
    {
        private readonly IFreehandGoalsService _goalService;
        private readonly IFreehandMatchService _matchService;
        private readonly IMapper _mapper;

        public FreehandGoalsController(IFreehandGoalsService goalService, IFreehandMatchService matchService, IMapper mapper)
        {
            _mapper = mapper;
            _goalService = goalService;
            _matchService = matchService;
        }

        [HttpGet("goals/{matchId}")]
        public ActionResult<IEnumerable<FreehandGoalReadDto>> GetFreehandGoalsByMatchId()
        {
            string matchId = RouteData.Values["matchId"].ToString();
            string userId = User.Identity.Name;
            bool access = _matchService.CheckFreehandMatchPermission(int.Parse(matchId), int.Parse(userId));

            if (!access)
                return Forbid();

            var allGoals = _goalService.GetFreehandGoalsByMatchId(int.Parse(matchId), int.Parse(userId));

            if (allGoals != null)
                return Ok(_mapper.Map<IEnumerable<FreehandGoalReadDto>>(allGoals));

            return NotFound();
        }

        [HttpGet("{goalId}", Name = "GetFreehandGoalById")]
        public ActionResult<FreehandGoalReadDto> GetFreehandGoalById(int matchId)
        {
            string goalId = RouteData.Values["goalId"].ToString();
            string userId = User.Identity.Name;

            bool matchPermission = _matchService.CheckFreehandMatchPermission(matchId, int.Parse(userId));

            if (!matchPermission)
                return Forbid();

            bool goalPermission = _goalService.CheckGoalPermission(int.Parse(userId), matchId, int.Parse(goalId));

            if (!goalPermission)
                return Forbid();

            var allMatches = _goalService.GetFreehandGoalById(int.Parse(goalId));

            if (allMatches == null)
                return NotFound();
                
            return Ok(_mapper.Map<FreehandGoalReadDto>(allMatches));
        }

        [HttpPost("{matchId}")]
        public ActionResult CreateFreehandGoal([FromBody] FreehandGoalCreateDto freehandGoalCreateDto)
        {
            string matchId = RouteData.Values["matchId"].ToString();
            string userId = User.Identity.Name;

            bool access = _matchService.CheckFreehandMatchPermission(int.Parse(matchId), int.Parse(userId));

            if (!access)
                return Forbid();

            var newGoal = _goalService.CreateFreehandGoal(int.Parse(userId), freehandGoalCreateDto);

            var freehandGoalReadDto = _mapper.Map<FreehandGoalReadDto>(newGoal);

            return CreatedAtRoute("GetFreehandGoalById", new { goalId = newGoal.Id }, freehandGoalReadDto);

        }

        [HttpDelete("{goalId}")]
        public ActionResult DeleteFeehandGoal(int goalID, int matchId)
        {
            string userId = User.Identity.Name;
            var goalItem = _goalService.GetFreehandGoalById(goalID);
            if (goalItem == null)
                return NotFound();

            bool hasPermission = _matchService.CheckFreehandMatchPermission(matchId, int.Parse(userId));

            if (!hasPermission)
                return Forbid();

            _goalService.DeleteFreehandGoal(goalItem);

            return NoContent();
        }

        [HttpPatch]
        public ActionResult UpdateFreehandGoal(int goalID, int matchID, JsonPatchDocument<FreehandGoalUpdateDto> patchDoc)
        {
            string userId = User.Identity.Name;
            var goalItem = _goalService.GetFreehandGoalById(goalID);
            if (goalItem == null)
                return NotFound();

            bool hasPermission = _matchService.CheckFreehandMatchPermission(matchID, int.Parse(userId));

            if (!hasPermission)
                return Forbid();

            var freehandGoalToPatch = _mapper.Map<FreehandGoalUpdateDto>(goalItem);
            patchDoc.ApplyTo(freehandGoalToPatch, ModelState);

            if (!TryValidateModel(freehandGoalToPatch))
                return ValidationProblem(ModelState);

            _mapper.Map(freehandGoalToPatch, goalItem);

            _goalService.UpdateFreehandGoal(goalItem);

            _goalService.SaveChanges();

            return NoContent();
        }
    }
}
