using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Matches;
using FoosballApi.Models.Matches;
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
    public class FreehandMatchesController : ControllerBase
    {
        private readonly IFreehandMatchService _matchService;
        private readonly IMapper _mapper;

        public FreehandMatchesController(IFreehandMatchService matchService, IMapper mapper)
        {
            _mapper = mapper;
            _matchService = matchService;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<FreehandMatchesReadDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FreehandMatchesReadDto>> GetAllFreehandMatchesByUser()
        {
            try
            {
                string userId = User.Identity.Name;

                var allMatches = _matchService.GetAllFreehandMatches(int.Parse(userId));

                if (allMatches == null)
                    return NotFound();

                return Ok(_mapper.Map<IEnumerable<FreehandMatchesReadDto>>(allMatches));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }

        [HttpGet("{matchId}", Name = "GetFreehandMatchById")]
        [ProducesResponseType(typeof(FreehandMatchesReadDto), StatusCodes.Status200OK)]
        public ActionResult<FreehandMatchesReadDto> GetFreehandMatchById()
        {
            try
            {
                string matchId = RouteData.Values["matchId"].ToString();
                string userId = User.Identity.Name;

                bool hasPermission = _matchService.CheckFreehandMatchPermission(int.Parse(matchId), int.Parse(userId));

                if (!hasPermission)
                    return Forbid();

                var allMatches = _matchService.GetFreehandMatchById(int.Parse(matchId));

                if (allMatches == null)
                    return NotFound();

                return Ok(_mapper.Map<FreehandMatchesReadDto>(allMatches));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost()]
        [ProducesResponseType(typeof(FreehandMatchCreateResultDto), StatusCodes.Status201Created)]
        public ActionResult CreateFreehandMatch([FromBody] FreehandMatchCreateDto freehandMatchCreateDto)
        {
            try
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;
                
                if (freehandMatchCreateDto.PlayerOneId != int.Parse(userId) && freehandMatchCreateDto.PlayerTwoId != int.Parse(userId))
                {
                    return Forbid();
                }

                FreehandMatchModel newMatch = _matchService.CreateFreehandMatch(int.Parse(userId), int.Parse(currentOrganisationId), freehandMatchCreateDto);

                var freehandMatchesReadDto = _mapper.Map<FreehandMatchCreateResultDto>(newMatch);

                return CreatedAtRoute("GetFreehandMatchById", new { matchId = newMatch.Id }, freehandMatchesReadDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult UpdateFreehandMatch(int matchId, JsonPatchDocument<FreehandMatchUpdateDto> patchDoc)
        {
            try
            {
                string userId = User.Identity.Name;
                var matchItem = _matchService.GetFreehandMatchByIdFromDatabase(matchId);
                if (matchItem == null)
                    return NotFound();

                bool hasPermission = _matchService.CheckFreehandMatchPermission(matchId, int.Parse(userId));

                if (!hasPermission)
                    return Forbid();

                var freehandMatchToPatch = _mapper.Map<FreehandMatchUpdateDto>(matchItem);
                patchDoc.ApplyTo(freehandMatchToPatch, ModelState);

                if (!TryValidateModel(freehandMatchToPatch))
                    return ValidationProblem(ModelState);

                _mapper.Map(freehandMatchToPatch, matchItem);

                _matchService.UpdateFreehandMatch(matchItem);

                _matchService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{matchId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteFreehandMatchById(int matchId)
        {
            try
            {
                string userId = User.Identity.Name;
                var matchItem = _matchService.GetFreehandMatchByIdFromDatabase(matchId);
                if (matchItem == null)
                    return NotFound();

                bool hasPermission = _matchService.CheckFreehandMatchPermission(matchId, int.Parse(userId));

                if (!hasPermission)
                    return Forbid();

                _matchService.DeleteFreehandMatch(matchItem);

                _matchService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
