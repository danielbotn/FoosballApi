using System;
using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Organisations;
using FoosballApi.Helpers;
using FoosballApi.Models.Organisations;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganisationsController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;
        private readonly IMapper _mapper;

        public OrganisationsController(IOrganisationService organisationService, IMapper mapper)
        {
            _organisationService = organisationService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrganisationReadDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<OrganisationReadDto>> GetAllOrganisations()
        {
            try
            {
                var orgItems = _organisationService.GettAllOrganisations();

                return Ok(_mapper.Map<IEnumerable<OrganisationReadDto>>(orgItems));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrganisationReadDto), StatusCodes.Status201Created)]
        public ActionResult CreateOrganisation([FromBody] OrganisationModelCreate organisationModel)
        {
            try
            {
                string userId = User.Identity.Name;

                int organisationId = _organisationService.CreateOrganisation(organisationModel, int.Parse(userId));

                var organisationReadDto = _mapper.Map<OrganisationReadDto>(organisationModel);

                return CreatedAtRoute("getOrganisationById", new { Id = organisationId }, organisationReadDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id}", Name = "getOrganisationById")]
        [ProducesResponseType(typeof(OrganisationReadDto), StatusCodes.Status200OK)]
        public ActionResult<OrganisationReadDto> GetOrganisationById(int id)
        {
            try
            {
                var userItem = _organisationService.GetOrganisationById(id);

                if (userItem == null)
                    return NotFound();

                return Ok(_mapper.Map<OrganisationReadDto>(userItem));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult PartialOrganisationUpdate(int id, JsonPatchDocument<OrganisationUpdateDto> patchDoc)
        {
            try
            {
                var orgItem = _organisationService.GetOrganisationById(id);

                if (orgItem == null)
                    return NotFound();

                string userId = User.Identity.Name;

                if (int.Parse(userId) != id)
                    return Forbid();

                var organisationToPatch = _mapper.Map<OrganisationUpdateDto>(orgItem);
                patchDoc.ApplyTo(organisationToPatch, ModelState);

                if (!TryValidateModel(organisationToPatch))
                    return ValidationProblem(ModelState);

                _mapper.Map(organisationToPatch, orgItem);

                _organisationService.UpdateOrganisation(orgItem);

                _organisationService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("user")]
        [ProducesResponseType(typeof(OrganisationReadDto), StatusCodes.Status200OK)]
        public ActionResult<OrganisationReadDto> GetOrganisationsByUser(int id)
        {
            try
            {
                var userItem = _organisationService.GetOrganisationsByUser(id);

                return Ok(_mapper.Map<IEnumerable<OrganisationReadDto>>(userItem));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteOrganisation(int id)
        {
            try
            {
                var organisation = _organisationService.GetOrganisationById(id);

                if (organisation == null)
                    return NotFound();

                _organisationService.DeleteOrganisation(organisation);

                _organisationService.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
