using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Leagues;
using FoosballApi.Models.Leagues;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IMapper _mapper;

        public LeaguesController(ILeagueService leagueService, IMapper mapper)
        {
            _leagueService = leagueService;
            _mapper = mapper;
        }

        [HttpGet("organisation")]
        public ActionResult<IEnumerable<LeagueReadDto>> GetLeaguesByOrganisation(int organisationId)
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

        [HttpGet("{id}")]
        public ActionResult<LeagueReadDto> GetLeagueById()
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

        [HttpPatch("{id}")]
        public ActionResult PartialLeagueUpdate(int id, JsonPatchDocument<LeagueUpdateDto> patchDoc)
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

        [HttpGet("league-players")]
        public ActionResult<IEnumerable<LeaguePlayersReadDto>> GetLeaguePlayers(int leagueId)
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

        [HttpPost()]
        public IActionResult CreateLeague([FromBody] LeagueModelCreate leagueModelCreate)
        {
            DateTime now = DateTime.Now;
            LeagueModel leagueModel = new LeagueModel();
            leagueModel.Name = leagueModelCreate.Name;
            leagueModel.OrganisationId = leagueModelCreate.OrganisationId;
            leagueModel.TypeOfLeague = leagueModelCreate.TypeOfLeague;
            leagueModel.UpTo = leagueModelCreate.UpTo;
            leagueModel.Created_at = now;

            int userId = int.Parse(User.Identity.Name);
            bool hasAccess = _leagueService.CheckLeagueAccess(userId, leagueModelCreate.OrganisationId);

            if (!hasAccess)
                return Forbid();

            _leagueService.CreateLeague(leagueModel);

            // TODO CreatedAtRoute
            return Ok();
        }

        [HttpDelete("")]
        public ActionResult DeleteLeagueById(int leagueId)
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
    }
}
