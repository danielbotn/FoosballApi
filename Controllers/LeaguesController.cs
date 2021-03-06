using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.DoubleLeagueMatches;
using FoosballApi.Dtos.Leagues;
using FoosballApi.Dtos.SingleLeagueMatches;
using FoosballApi.Models.Leagues;
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
    public class LeaguesController : ControllerBase
    {
        private readonly ILeagueService _leagueService;
        private readonly ISingleLeagueMatchService _singleLeagueMatchService;
        private readonly IDoubleLeaugeMatchService _doubleLeagueMatchService;
        private readonly IMapper _mapper;

        public LeaguesController(
            ILeagueService leagueService,
            ISingleLeagueMatchService singleLeagueMatchService,
            IMapper mapper,
            IDoubleLeaugeMatchService doubleLeagueMatchService)
        {
            _leagueService = leagueService;
            _singleLeagueMatchService = singleLeagueMatchService;
            _doubleLeagueMatchService = doubleLeagueMatchService;
            _mapper = mapper;
        }

        [HttpGet("organisation")]
        [ProducesResponseType(typeof(List<LeagueReadDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<LeagueReadDto>> GetLeaguesByOrganisation(int organisationId)
        {
            try
            {
                string userId = User.Identity.Name;
                var leagues = _leagueService.GetLeaguesByOrganisation(organisationId);

                bool hasAccess = _leagueService.CheckLeagueAccess(int.Parse(userId), organisationId);

                if (!hasAccess)
                    return Forbid();

                if (leagues == null)
                    return NotFound();

                return Ok(_mapper.Map<IEnumerable<LeagueReadDto>>(leagues));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LeagueReadDto), StatusCodes.Status200OK)]
        public ActionResult<LeagueReadDto> GetLeagueById()
        {
            try
            {
                string leagueId = RouteData.Values["id"].ToString();
                string userId = User.Identity.Name;

                int organisationId = _leagueService.GetOrganisationId(int.Parse(leagueId));

                bool hasAccess = _leagueService.CheckLeagueAccess(int.Parse(userId), organisationId);

                if (!hasAccess)
                    return Forbid();

                LeagueModel leagueModel = _leagueService.GetLeagueById(int.Parse(leagueId));

                if (leagueModel == null)
                    return NotFound();

                return Ok(_mapper.Map<LeagueReadDto>(leagueModel));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult PartialLeagueUpdate(int id, JsonPatchDocument<LeagueUpdateDto> patchDoc)
        {
            try
            {
                var leagueItem = _leagueService.GetLeagueById(id);

                if (leagueItem == null)
                    return NotFound();

                string userId = User.Identity.Name;

                if (int.Parse(userId) != id)
                    return Forbid();

                var leagueToPatch = _mapper.Map<LeagueUpdateDto>(leagueItem);
                patchDoc.ApplyTo(leagueToPatch, ModelState);

                if (!TryValidateModel(leagueToPatch))
                    return ValidationProblem(ModelState);

                _mapper.Map(leagueToPatch, leagueItem);

                _leagueService.UpdateLeague(leagueItem);

                _leagueService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("league-players")]
        [ProducesResponseType(typeof(List<LeaguePlayersReadDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<LeaguePlayersReadDto>> GetLeaguePlayers(int leagueId)
        {
            try
            {
                string userId = User.Identity.Name;
                var leaguePlayers = _leagueService.GetLeaguesPlayers(leagueId);

                int organisationId = _leagueService.GetOrganisationId(leagueId);

                bool hasAccess = _leagueService.CheckLeagueAccess(int.Parse(userId), organisationId);

                if (!hasAccess)
                    return Forbid();

                if (leaguePlayers == null)
                    return NotFound();

                return Ok(_mapper.Map<IEnumerable<LeaguePlayersReadDto>>(leaguePlayers));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost()]
        public IActionResult CreateLeague([FromBody] LeagueModelCreate leagueModelCreate)
        {
            try
            {
                int userId = int.Parse(User.Identity.Name);
                bool hasAccess = _leagueService.CheckLeagueAccess(userId, leagueModelCreate.OrganisationId);

                if (!hasAccess)
                    return Forbid();

                _leagueService.CreateLeague(leagueModelCreate);

                // TODO CreatedAtRoute
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{leagueId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteLeagueById(int leagueId)
        {
            try
            {
                string userId = User.Identity.Name;
                LeagueModel league = _leagueService.GetLeagueById(leagueId);
                if (league == null)
                    return NotFound();

                bool hasAccess = _leagueService.CheckLeagueAccess(int.Parse(userId), league.OrganisationId);

                if (!hasAccess)
                    return Forbid();

                _leagueService.DeleteLeague(league);

                _leagueService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("single-league/standings")]
        [ProducesResponseType(typeof(List<SingleLeagueStandingsReadDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SingleLeagueStandingsReadDto>> GetSingleLeagueStandings(int leagueId)
        {
            try
            {
                string userId = User.Identity.Name;
                bool permission = _singleLeagueMatchService.CheckLeaguePermission(leagueId, int.Parse(userId));

                if (!permission)
                    return Forbid();

                var standings = _singleLeagueMatchService.GetSigleLeagueStandings(leagueId);

                return Ok(_mapper.Map<IEnumerable<SingleLeagueStandingsReadDto>>(standings));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("double-league/standings")]
        [ProducesResponseType(typeof(List<SingleLeagueStandingsReadDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<DoubleLeagueStandingsReadDto>> GetDoubleLeagueStandings(int leagueId)
        {
            try
            {
                string userId = User.Identity.Name;
                bool permission = _doubleLeagueMatchService.CheckLeaguePermission(leagueId, int.Parse(userId));

                if (!permission)
                    return Forbid();

                var standings = _doubleLeagueMatchService.GetDoubleLeagueStandings(leagueId);

                return Ok(_mapper.Map<IEnumerable<DoubleLeagueStandingsReadDto>>(standings));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
