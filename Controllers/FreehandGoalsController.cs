using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Goals;
using FoosballApi.Models.Goals;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FreehandGoalsController : ControllerBase
    {
        private readonly IFreehandGoalsService _goalService;
        private readonly IFreehandMatchService _matchService;
        private readonly IMapper _mapper;

        public FreehandGoalsController(IFreehandGoalsService goalService, IFreehandMatchService matchService, IMapper mapper)
        {
            _mapper = mapper;
            _goalService = goalService;
            _matchService = matchService;
        }

        [HttpGet("{matchId}")]
        public ActionResult<IEnumerable<FreehandGoalsReadDto>> GetFreehandGoalsByMatchId()
        {
            string matchId = RouteData.Values["matchId"].ToString();
            string userId = User.Identity.Name;
            bool access = _matchService.CheckFreehandMatchPermission(int.Parse(matchId), int.Parse(userId));

            if (!access)
                return Forbid();

            var allGoals = _goalService.GetFreehandGoalsByMatchId(int.Parse(matchId), int.Parse(userId));
            
            if (allGoals != null)
                return Ok(_mapper.Map<IEnumerable<FreehandGoalsReadDto>>(allGoals));

            return NotFound();
        }

    }
}
