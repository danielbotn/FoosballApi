using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.DoubleGoals;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FreehandDoubleGoalsController : ControllerBase
    {
        private readonly IFreehandDoubleGoalService _doubleFreehandGoalservice;
        private readonly IFreehandDoubleMatchService _doubleFreehandMatchService;
        private readonly IMapper _mapper;

        public FreehandDoubleGoalsController(IFreehandDoubleGoalService doubleFreehandGoalservice, IMapper mapper, IFreehandDoubleMatchService doubleFreehandMatchService)
        {
            _mapper = mapper;
            _doubleFreehandGoalservice = doubleFreehandGoalservice;
            _doubleFreehandMatchService = doubleFreehandMatchService;
        }

        [HttpGet("goals/{matchId}")]
        [ProducesResponseType(typeof(List<FreehandDoubleGoalsExtendedDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FreehandDoubleGoalsExtendedDto>> GetFreehandDoubleGoalsByMatchId()
        {
            try
            {
                string matchId = RouteData.Values["matchId"].ToString();
                string userId = User.Identity.Name;

                bool access = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), int.Parse(matchId));

                if (!access)
                    return Forbid();

                var allGoals = _doubleFreehandGoalservice.GetAllFreehandGoals(int.Parse(matchId), int.Parse(userId));

                if (allGoals == null)
                    return NotFound();

                return Ok(_mapper.Map<IEnumerable<FreehandDoubleGoalsExtendedDto>>(allGoals));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{goalId}", Name = "GetFreehandDoubleGoalById")]
        [ProducesResponseType(typeof(FreehandDoubleGoalReadDto), StatusCodes.Status200OK)]
        public ActionResult<FreehandDoubleGoalReadDto> GetFreehandDoubleGoalById(int goalId, int matchId)
        {
            try
            {
                string userId = User.Identity.Name;

                bool matchAccess = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), matchId);

                if (!matchAccess)
                    return Forbid();

                bool goalAccess = _doubleFreehandGoalservice.CheckGoalPermission(int.Parse(userId), matchId, goalId);

                if (!goalAccess)
                    return Forbid();

                var freehandDoubleGoal = _doubleFreehandGoalservice.GetFreehandDoubleGoal(goalId);

                return Ok(_mapper.Map<FreehandDoubleGoalReadDto>(freehandDoubleGoal));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(FreehandDoubleGoalReadDto), StatusCodes.Status201Created)]
        public ActionResult CreateFreehandDoubleGoal([FromBody] FreehandDoubleGoalCreateDto freehandGoalCreateDto)
        {
            try
            {
                int matchId = freehandGoalCreateDto.DoubleMatchId;
                string userId = User.Identity.Name;

                bool matchAccess = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), matchId);

                if (!matchAccess)
                    return Forbid();

                var newGoal = _doubleFreehandGoalservice.CreateDoubleFreehandGoal(int.Parse(userId), freehandGoalCreateDto);

                var freehandGoalReadDto = _mapper.Map<FreehandDoubleGoalReadDto>(newGoal);

                return CreatedAtRoute("GetFreehandDoubleGoalById", new { goalId = newGoal.Id }, freehandGoalReadDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{matchId}/{goalId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteDoubleFreehandGoal(string goalId, string matchId)
        {
            try
            {
                string userId = User.Identity.Name;
                var goalItem = _doubleFreehandGoalservice.GetFreehandDoubleGoal(int.Parse(goalId));
                if (goalItem == null)
                    return NotFound();

                bool hasPermission = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), int.Parse(matchId));

                if (!hasPermission)
                    return Forbid();

                _doubleFreehandGoalservice.DeleteFreehandGoal(goalItem);

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult UpdateFreehandDoubleGoal(int goalId, int matchId, JsonPatchDocument<FreehandDoubleGoalUpdateDto> patchDoc)
        {
            try
            {
                string userId = User.Identity.Name;
                var goalItem = _doubleFreehandGoalservice.GetFreehandDoubleGoal(goalId);
                if (goalItem == null)
                    return NotFound();

                bool matchPermission = _doubleFreehandMatchService.CheckMatchPermission(int.Parse(userId), matchId);

                if (!matchPermission)
                    return Forbid();

                bool goalPermission = _doubleFreehandGoalservice.CheckGoalPermission(int.Parse(userId), matchId, goalId);

                if (!goalPermission)
                    return Forbid();

                var freehandGoalToPatch = _mapper.Map<FreehandDoubleGoalUpdateDto>(goalItem);
                patchDoc.ApplyTo(freehandGoalToPatch, ModelState);

                if (!TryValidateModel(freehandGoalToPatch))
                    return ValidationProblem(ModelState);

                _mapper.Map(freehandGoalToPatch, goalItem);

                _doubleFreehandGoalservice.UpdateFreehanDoubledGoal(goalItem);

                _doubleFreehandGoalservice.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}