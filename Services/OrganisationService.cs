using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;

namespace FoosballApi.Services
{
    public interface IOrganisationService
    {
        IEnumerable<OrganisationModel> GettAllOrganisations();

        OrganisationModel GetOrganisationById(int id);

        IEnumerable<OrganisationModel> GetOrganisationsByUser(int id);

        void UpdateOrganisation(OrganisationModel organisation);

        void DeleteOrganisation(OrganisationModel organisation);

        bool SaveChanges();
    }
    public class OrganisationService : IOrganisationService
    {
        private readonly DataContext _context;

        public OrganisationService(DataContext context)
        {
            _context = context;
        }

        public OrganisationModel GetOrganisationById(int id)
        {
            return _context.Organisations.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<OrganisationModel> GetOrganisationsByUser(int id)
        {
            var query = from o in _context.Organisations
                        join ol in _context.OrganisationList on o.Id equals ol.OrganisationId
                        where ol.UserId == id
                        select o;

            var data = query.ToList();

            return data;
        }

        public IEnumerable<OrganisationModel> GettAllOrganisations()
        {
            return _context.Organisations.ToList();
        }

        public void UpdateOrganisation(OrganisationModel organisation)
        {
            // Do nothing
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void DeleteOrganisation(OrganisationModel organisation)
        {
            if (organisation == null)
            {
                throw new ArgumentNullException(nameof(organisation));
            }
            _context.Organisations.Remove(organisation);
        }
    }
}
