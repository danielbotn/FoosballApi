using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Matches;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
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
        public ActionResult<IEnumerable<FreehandMatchesReadDto>> GetAllFreehandMatchesByUser()
        {
            string userId = User.Identity.Name;

            var allMatches = _matchService.GetAllFreehandMatches(int.Parse(userId));

            if (allMatches != null)
            {
                return Ok(_mapper.Map<IEnumerable<FreehandMatchesReadDto>>(allMatches));
            }

            return NotFound();
        }

        [HttpGet("{matchId}", Name = "GetFreehandMatchById")]
        public ActionResult<FreehandMatchesReadDto> GetFreehandMatchById()
        {
            string matchId = RouteData.Values["matchId"].ToString();
            string userId = User.Identity.Name;

            bool hasPermission = _matchService.CheckFreehandMatchPermission(int.Parse(matchId), int.Parse(userId));

            if (!hasPermission)
                return Forbid();

            var allMatches = _matchService.GetFreehandMatchById(int.Parse(matchId));

            if (allMatches != null)
                return Ok(_mapper.Map<FreehandMatchesReadDto>(allMatches));

            return NotFound();
        }

        [HttpPost("freehand-match")]
        public ActionResult CreateFreehandMatch([FromBody] FreehandMatchCreateDto organisationModel)
        {
            string userId = User.Identity.Name;

            var newMatch = _matchService.CreateFreehandMatch(int.Parse(userId), organisationModel);

            var freehandMatchesReadDto = _mapper.Map<FreehandMatchesReadDto>(newMatch);

            return CreatedAtRoute("GetFreehandMatchById", new { matchId = newMatch.Id }, freehandMatchesReadDto);
        }

        [HttpPatch("freehand-match")]
        public ActionResult UpdateFreehandMatch(int matchId, JsonPatchDocument<FreehandMatchUpdateDto> patchDoc)
        {
            string userId = User.Identity.Name;
            var matchItem = _matchService.GetFreehandMatchById(matchId);
            if (matchItem == null)
            {
                return NotFound();
            }

            bool hasPermission = _matchService.CheckFreehandMatchPermission(matchId, int.Parse(userId));

            if (!hasPermission)
                return Forbid();

            var freehandMatchToPatch = _mapper.Map<FreehandMatchUpdateDto>(matchItem);
            patchDoc.ApplyTo(freehandMatchToPatch, ModelState);

            if (!TryValidateModel(freehandMatchToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(freehandMatchToPatch, matchItem);

            _matchService.UpdateFreehandMatch(matchItem);

            _matchService.SaveChanges();

            return NoContent();
        }

        [HttpDelete("freehand-match")]
        public ActionResult DeleteFreehandMatchById(int matchId)
        {
            string userId = User.Identity.Name;
            var matchItem = _matchService.GetFreehandMatchById(matchId);
            if (matchItem == null)
                return NotFound();

            bool hasPermission = _matchService.CheckFreehandMatchPermission(matchId, int.Parse(userId));

            if (!hasPermission)
                return Forbid();

            _matchService.DeleteFreehandMatch(matchItem);

            _matchService.SaveChanges();

            return NoContent();
        }
    }
}
