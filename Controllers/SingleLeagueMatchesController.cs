using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.SingleLeagueMatches;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
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
        public ActionResult<IEnumerable<SingleLeagueMatchesReadDto>> GetAllSingleLeaguesMatchesByOrganisationId(int leagueId)
        {
            string userId = User.Identity.Name;
            string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

            bool permission = _singleLeagueMatchService.CheckLeaguePermission(leagueId, int.Parse(userId));

            if (!permission)
                return Forbid();

            var allMatches = _singleLeagueMatchService.GetAllMatchesByOrganisationId(int.Parse(currentOrganisationId), leagueId);

            return Ok(allMatches);

        }
    }
}