using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.DoubleMatches;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FreehandDoubleMatchesController : ControllerBase
    {
        private readonly IFreehandDoubleMatchService _doubleMatchService;
        private readonly IMapper _mapper;

        public FreehandDoubleMatchesController(IFreehandDoubleMatchService doubleMatchService, IMapper mapper)
        {
            _mapper = mapper;
            _doubleMatchService = doubleMatchService;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<FreehandDoubleMatchReadDto>> GetAllFreehandDoubleMatchesByUser()
        {
            string userId = User.Identity.Name;

            var allMatches = _doubleMatchService.GetAllFreehandDoubleMatches(int.Parse(userId));

            if (allMatches != null)
            {
                return Ok(_mapper.Map<IEnumerable<FreehandDoubleMatchReadDto>>(allMatches));
            }

            return NotFound();
        }
    }
}