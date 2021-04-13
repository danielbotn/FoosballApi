using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Leagues;
using FoosballApi.Models.Leagues;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaguesController : ControllerBase
    {
        private readonly ILeagueService _leagueService;
        private readonly IMapper _mapper;

        public LeaguesController(ILeagueService leagueService, IMapper mapper)
        {
            _leagueService = leagueService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("{organisationId}")]
        public ActionResult<IEnumerable<LeagueReadDto>> GetLeaguesByOrganisation()
        {
            string userId = User.Identity.Name;
            string organisationId = RouteData.Values["organisationId"].ToString();
            int number;
            bool isNumber = int.TryParse(organisationId, out number);

            if (!isNumber)
                return UnprocessableEntity();

            var leagues = _leagueService.GetLeaguesByOrganisation(int.Parse(organisationId));

            bool hasAccess = _leagueService.CheckLeagueAccess(int.Parse(userId), int.Parse(organisationId));

            if (!hasAccess)
                return Forbid();

            if (leagues != null)
            {
                return Ok(_mapper.Map<IEnumerable<LeagueReadDto>>(leagues));
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet("leaguePlayers")]
        public ActionResult<IEnumerable<LeaguePlayersReadDto>> GetLeaguePlayers(int leagueId)
        {
            string userId = User.Identity.Name;
            var leaguePlayers = _leagueService.GetLeaguesPlayers(leagueId);

            int organisationId = _leagueService.GetOrganisationId(leagueId);

            bool hasAccess = _leagueService.CheckLeagueAccess(int.Parse(userId), organisationId);

            if (!hasAccess)
                return Forbid();

            if (leaguePlayers != null)
            {
                return Ok(_mapper.Map<IEnumerable<LeaguePlayersReadDto>>(leaguePlayers));
            }

            return NotFound();
        }

        [HttpPost()]
        public IActionResult CreateLeague([FromBody] LeagueModelCreate leagueModelCreate)
        {
            LeagueModel leagueModel = new LeagueModel();
            leagueModel.Name = leagueModelCreate.Name;
            leagueModel.OrganisationId = leagueModelCreate.OrganisationId;
            leagueModel.TypeOfLeague = leagueModelCreate.TypeOfLeague;
            leagueModel.UpTo = leagueModelCreate.UpTo;

            int userId = int.Parse(User.Identity.Name);
            bool hasAccess = _leagueService.CheckLeagueAccess(userId, leagueModelCreate.OrganisationId);

            if (!hasAccess)
                return Forbid();

            _leagueService.CreateLeague(leagueModel);

            // TODO CreatedAtRoute
            return Ok();
        }

    }
}