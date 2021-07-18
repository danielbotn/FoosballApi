using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FoosballApi.Dtos.SingleLeagueMatches;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SingleLeagueMatchesController : ControllerBase
    {
        private readonly ISingleLeagueMatchService _singleLeagueMatchService;
        private readonly IMapper _mapper;

        public SingleLeagueMatchesController(ISingleLeagueMatchService singleLeagueMatchService, IMapper mapper)
        {
            _mapper = mapper;
            _singleLeagueMatchService = singleLeagueMatchService;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<SingleLeagueMatchReadDto>> GetAllSingleLeaguesMatchesByOrganisationId(int leagueId)
        {
            string userId = User.Identity.Name;
            string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

            bool permission = _singleLeagueMatchService.CheckLeaguePermission(leagueId, int.Parse(userId));

            if (!permission)
                return Forbid();

            var allMatches = _singleLeagueMatchService.GetAllMatchesByOrganisationId(int.Parse(currentOrganisationId), leagueId);

            return Ok(allMatches);
        }

        [HttpGet("{matchId}", Name = "GetSingleLeagueMatchById")]
        public ActionResult<SingleLeagueMatchReadDto> GetSingleLeagueMatchById(int matchId)
        {
            string userId = User.Identity.Name;

            bool hasPermission = _singleLeagueMatchService.CheckMatchPermission(matchId, int.Parse(userId));

            if (!hasPermission)
                return Forbid();

            var match = _singleLeagueMatchService.GetSingleLeagueMatchById(matchId);

            if (match == null)
                return NotFound();

            return Ok(_mapper.Map<SingleLeagueMatchReadDto>(match));
        }
        
        // TO DO Remove this code. To use dapper see how this endpoints works
        // [HttpGet("test-dapper")]
        // public async Task<ActionResult> TestDapper()
        // {
        //     CancellationToken ct = new();
        //     var gaur = await _singleLeagueMatchService.TestDapper(ct);

        //     return Ok(gaur);
        // }

        [HttpPatch("")]
        public ActionResult UpdateSingleLeagueMatch(int matchId, JsonPatchDocument<SingleLeagueMatchUpdateDto> patchDoc)
        {
            var match = _singleLeagueMatchService.GetSingleLeagueMatchById(matchId);

            if (match == null)
                return NotFound();

            string userId = User.Identity.Name;
            bool hasPermission = _singleLeagueMatchService.CheckMatchPermission(matchId, int.Parse(userId));

            if (!hasPermission)
                return Forbid();

            var matchToPatch = _mapper.Map<SingleLeagueMatchUpdateDto>(match);
            patchDoc.ApplyTo(matchToPatch, ModelState);

            if (!TryValidateModel(matchToPatch))
                return ValidationProblem(ModelState);

            _mapper.Map(matchToPatch, match);

            _singleLeagueMatchService.UpdateSingleLeagueMatch(match);

            _singleLeagueMatchService.SaveChanges();

            return NoContent();
        }
    }
}
