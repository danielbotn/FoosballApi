using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Models;
using FoosballApi.Models.Organisations;

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

        int CreateOrganisation(OrganisationModelCreate organisation);
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

        public int CreateOrganisation(OrganisationModelCreate organisationModel)
        {
            OrganisationModel om = new OrganisationModel();
            DateTime now = DateTime.Now;
            om.Name = organisationModel.Name;
            om.Type = organisationModel.Type;
            om.CreatedAt = now;

            _context.Organisations.Add(om);
            _context.SaveChanges();

            int id = om.Id;
            OrganisationListModel olm = new OrganisationListModel();
            olm.OrganisationId = id;
            olm.UserId = organisationModel.UserId;

            _context.OrganisationList.Add(olm);
            _context.SaveChanges();

            return id;
        }
    }
}
