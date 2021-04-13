using System.Collections.Generic;
using AutoMapper;
using FoosballApi.Dtos.Organisations;
using FoosballApi.Helpers;
using FoosballApi.Models.Organisations;
using FoosballApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationsController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;
        private readonly IMapper _mapper;

        public OrganisationsController(IOrganisationService organisationService, IMapper mapper)
        {
            _organisationService = organisationService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<OrganisationReadDto>> GetAllOrganisations()
        {
            var orgItems = _organisationService.GettAllOrganisations();
            return Ok(_mapper.Map<IEnumerable<OrganisationReadDto>>(orgItems));
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateOrganisation([FromBody] OrganisationModelCreate organisationModel)
        {
            string userId = User.Identity.Name;
            
            if (int.Parse(userId) != organisationModel.UserId) {
                return Forbid();
            }

            int organisationId = _organisationService.CreateOrganisation(organisationModel);

            var organisationReadDto = _mapper.Map<OrganisationReadDto>(organisationModel);
            
            return CreatedAtRoute("getOrganisationById", new { Id = organisationId }, organisationReadDto);
        }

        [Authorize]
        [HttpGet("{id}", Name = "getOrganisationById")]
        public ActionResult<OrganisationReadDto> GetOrganisationById(int id)
        {
            var userItem = _organisationService.GetOrganisationById(id);

            if (userItem != null)
            {
                return Ok(_mapper.Map<OrganisationReadDto>(userItem));
            }

            return NotFound();
        }

        [Authorize]
        [HttpPatch("{id}")]
        public ActionResult PartialUserUpdate(int id, JsonPatchDocument<OrganisationUpdateDto> patchDoc)
        {
            var orgItem = _organisationService.GetOrganisationById(id);
            if (orgItem == null)
            {
                return NotFound();
            }

            string userId = User.Identity.Name;

            if (int.Parse(userId) != id) {
                return Forbid();
            }

            var organisationToPatch = _mapper.Map<OrganisationUpdateDto>(orgItem);
            patchDoc.ApplyTo(organisationToPatch, ModelState);

            if (!TryValidateModel(organisationToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(organisationToPatch, orgItem);

            _organisationService.UpdateOrganisation(orgItem);

            _organisationService.SaveChanges();

            return NoContent();
        }

        [Authorize]
        [HttpGet("user")]
        public ActionResult<OrganisationReadDto> GetOrganisationsByUser(int id)
        {
            var userItem = _organisationService.GetOrganisationsByUser(id);

            return Ok(_mapper.Map<IEnumerable<OrganisationReadDto>>(userItem));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteOrganisation(int id)
        {
            var organisation = _organisationService.GetOrganisationById(id);

            if (organisation == null)
            {
                return NotFound();
            }

            string userId = User.Identity.Name;

            if (int.Parse(userId) != id) {
                return Forbid();
            }

            _organisationService.DeleteOrganisation(organisation);

            _organisationService.SaveChanges();

            return NoContent();
        }
    }
}
