using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Leagues;
using FoosballApi.Models.Leagues;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
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
    }
}