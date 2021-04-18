using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Matches;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly IMapper _mapper;

        public MatchesController(IMatchService matchService, IMapper mapper)
        {
            _mapper = mapper;
            _matchService = matchService;
        }

        [Authorize]
        [HttpGet("freehand-matches")]
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

        [Authorize]
        [HttpGet("freehand-matches/{matchId}", Name = "GetFreehandMatchById")]
        public ActionResult<FreehandMatchesReadDto> GetFreehandMatchById()
        {
            string matchId = RouteData.Values["matchId"].ToString();
            string userId = User.Identity.Name;

            bool hasPermission = _matchService.CheckFreehandMatchPermission(int.Parse(matchId), int.Parse(userId));

            if (!hasPermission)
                return Forbid();

            var allMatches = _matchService.GetFreehandMatchById(int.Parse(matchId));

            if (allMatches != null)
            {
                return Ok(_mapper.Map<FreehandMatchesReadDto>(allMatches));
                // return Ok(_mapper.Map<IEnumerable<FreehandMatchesReadDto>>(allMatches));
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost("freehand-match")]
        public ActionResult CreateFreehandMatch([FromBody] FreehandMatchCreateDto organisationModel)
        {
            string userId = User.Identity.Name;

            _matchService.CreateFreehandMatch(int.Parse(userId), organisationModel);

            var freehandMatchesCreateDto = _mapper.Map<FreehandMatchCreateDto>(organisationModel);

            // TO DO
            // return CreatedAtRoute("getOrganisationById", new { Id = organisationId }, organisationReadDto);
            return Ok();
        }
    }
}