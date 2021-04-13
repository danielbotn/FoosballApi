using System;
using System.ComponentModel.DataAnnotations;

namespace FoosballApi.Models.Organisations
{
    public class OrganisationModelCreate
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}