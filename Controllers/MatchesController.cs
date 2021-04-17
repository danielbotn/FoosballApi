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
    }
}