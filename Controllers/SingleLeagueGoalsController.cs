using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.SingleLeagueGoals;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SingleLeagueGoalsController : ControllerBase
    {
        private readonly ISingleLeagueGoalService _singleLeagueGoalService;
         private readonly ISingleLeagueMatchService _singleLeagueMatchService;
        private readonly IMapper _mapper;

        public SingleLeagueGoalsController(ISingleLeagueGoalService singleLeagueGoalService, ISingleLeagueMatchService singleLeagueMatchService, IMapper mapper)
        {
            _mapper = mapper;
            _singleLeagueGoalService = singleLeagueGoalService;
            _singleLeagueMatchService = singleLeagueMatchService;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<SingleLeagueGoalReadDto>> GetAllSingleLeagueGoalsByLeagueId(int leagueId, int matchId)
        {
            string userId = User.Identity.Name;
            string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

            bool permission = _singleLeagueMatchService.CheckLeaguePermission(leagueId, int.Parse(userId));

            if (!permission)
                return Forbid();

            var allGoals = _singleLeagueGoalService.GetAllSingleLeagueGoalsByLeagueId(matchId);

            return Ok(allGoals);
        }
    }
}