using System;
using System.Collections.Generic;
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
    public class FreehandDoubleMatchesController : ControllerBase
    {
        private readonly IFreehandDoubleMatchService _doubleMatchService;
        private readonly IMapper _mapper;

        public FreehandDoubleMatchesController(IFreehandDoubleMatchService doubleMatchService, IMapper mapper)
        {
            _mapper = mapper;
            _doubleMatchService = doubleMatchService;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<FreehandDoubleMatchReadDto>> GetAllFreehandDoubleMatchesByUser()
        {
            try
            {
                string userId = User.Identity.Name;

                var allMatches = _doubleMatchService.GetAllFreehandDoubleMatches(int.Parse(userId));

                if (allMatches == null)
                    return NotFound();

                return Ok(_mapper.Map<IEnumerable<FreehandDoubleMatchReadDto>>(allMatches));
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        [HttpGet("{matchId}", Name = "GetFreehandDoubleMatchByMatchId")]
        public ActionResult<FreehandDoubleMatchReadDto> GetFreehandDoubleMatchByMatchId()
        {
            try
            {
                string matchId = RouteData.Values["matchId"].ToString();
                string userId = User.Identity.Name;

                bool access = _doubleMatchService.CheckMatchPermission(int.Parse(userId), int.Parse(matchId));

                if (!access)
                    return Forbid();

                var match = _doubleMatchService.GetFreehandDoubleMatchById(int.Parse(matchId));

                if (match == null)
                    return NotFound();

                return Ok(_mapper.Map<FreehandDoubleMatchReadDto>(match));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost()]
        public ActionResult CreateDoubleFreehandMatch([FromBody] FreehandDoubleMatchCreateDto freehandDoubleMatchCreateDto)
        {
            string userId = User.Identity.Name;

            var newMatch = _doubleMatchService.CreateFreehandDoubleMatch(int.Parse(userId), freehandDoubleMatchCreateDto);

            var freehandDoubleMatchReadDto = _mapper.Map<FreehandDoubleMatchReadDto>(newMatch);

            return CreatedAtRoute("GetFreehandDoubleMatchByMatchId", new { matchId = newMatch.Id }, freehandDoubleMatchReadDto);
        }

        [HttpPatch()]
        public ActionResult UpdateDoubleFreehandMatch(int matchId, JsonPatchDocument<FreehandDoubleMatchUpdateDto> patchDoc)
        {
            try
            {
                string userId = User.Identity.Name;

                var matchItem = _doubleMatchService.GetFreehandDoubleMatchById(matchId);
                if (matchItem == null)
                    return NotFound();

                bool hasPermission = _doubleMatchService.CheckMatchPermission(int.Parse(userId), matchId);

                if (!hasPermission)
                    return Forbid();

                var freehandMatchToPatch = _mapper.Map<FreehandDoubleMatchUpdateDto>(matchItem);
                patchDoc.ApplyTo(freehandMatchToPatch, ModelState);

                if (!TryValidateModel(freehandMatchToPatch))
                    return ValidationProblem(ModelState);

                _mapper.Map(freehandMatchToPatch, matchItem);

                _doubleMatchService.UpdateFreehandMatch(matchItem);

                _doubleMatchService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{matchId}")]
        public ActionResult DeleteDoubleFreehandMatch(int matchId)
        {
            try
            {
                string userId = User.Identity.Name;
                var matchItem = _doubleMatchService.GetFreehandDoubleMatchById(matchId);
                if (matchItem == null)
                    return NotFound();

                bool hasPermission = _doubleMatchService.CheckMatchPermission(int.Parse(userId), matchId);

                if (!hasPermission)
                    return Forbid();

                _doubleMatchService.DeleteFreehandMatch(matchItem);

                _doubleMatchService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}