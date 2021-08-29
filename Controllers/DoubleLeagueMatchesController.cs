using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.DoubleLeagueMatches;
using FoosballApi.Models.DoubleLeagueMatches;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoubleLeagueMatchesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDoubleLeaugeMatchService _doubleLeaugeMatchService;

        public DoubleLeagueMatchesController(IMapper mapper, IDoubleLeaugeMatchService doubleLeaugeMatchService)
        {
            _mapper = mapper;
            _doubleLeaugeMatchService = doubleLeaugeMatchService;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<AllMatchesModelReadDto>> GetAllDoubleLeaguesMatchesByLeagueId(int leagueId)
        {
            try
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

                bool permission = _doubleLeaugeMatchService.CheckLeaguePermission(leagueId, int.Parse(userId));

                if (!permission)
                    return Forbid();

                var allMatches = _doubleLeaugeMatchService.GetAllMatchesByOrganisationId(int.Parse(currentOrganisationId), leagueId);

                return Ok(_mapper.Map<IEnumerable<AllMatchesModelReadDto>>(allMatches));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("matchId")]
        public ActionResult<AllMatchesModelReadDto> GetDoubleLeagueMatchById(int matchId)
        {
            try
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

                bool permission = _doubleLeaugeMatchService.CheckMatchAccess(matchId, int.Parse(userId), int.Parse(currentOrganisationId));

                if (!permission)
                    return Forbid();

                var matchData = _doubleLeaugeMatchService.GetDoubleLeagueMatchById(matchId);

                return Ok(_mapper.Map<AllMatchesModelReadDto>(matchData));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch("")]
        public ActionResult UpdateDoubleLeagueMatch(int matchId, JsonPatchDocument<DoubleLeagueMatchUpdateDto> patchDoc)
        {
            try
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;
                var match = _doubleLeaugeMatchService.GetDoubleLeagueMatchByIdSimple(matchId);

                if (match == null)
                    return NotFound();

                bool hasPermission = _doubleLeaugeMatchService.CheckMatchAccess(matchId, int.Parse(userId), int.Parse(currentOrganisationId));

                if (!hasPermission)
                    return Forbid();

                var matchToPatch = _mapper.Map<DoubleLeagueMatchUpdateDto>(match);
                patchDoc.ApplyTo(matchToPatch, ModelState);

                if (!TryValidateModel(matchToPatch))
                    return ValidationProblem(ModelState);

                _mapper.Map(matchToPatch, match);

                _doubleLeaugeMatchService.UpdateDoubleLeagueMatch(match);

                _doubleLeaugeMatchService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("reset-match")]
        public ActionResult ResetDoubleLeagueMatchById(int matchId)
        {
            try
            {
                string userId = User.Identity.Name;
                string currentOrganisationId = User.FindFirst("CurrentOrganisationId").Value;

                var matchItem = _doubleLeaugeMatchService.GetDoubleLeagueMatchByIdSimple(matchId);
                if (matchItem == null)
                    return NotFound();

                bool hasPermission = _doubleLeaugeMatchService.CheckMatchAccess(matchId, int.Parse(userId), int.Parse(currentOrganisationId));

                if (!hasPermission)
                    return Forbid();

                _doubleLeaugeMatchService.ResetMatch(matchItem, matchId);

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}