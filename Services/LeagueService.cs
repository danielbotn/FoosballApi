using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Models.Leagues;
using System.Linq;

namespace FoosballApi.Services
{
    public interface ILeagueService 
    {
        IEnumerable<LeagueModel> GetLeaguesByOrganisation(int organisationId);

        bool CheckLeagueAccess(int userId, int organisationId);
    }
    public class LeagueService : ILeagueService
    {
        private readonly DataContext _context;

        public LeagueService(DataContext context)
        {
            _context = context;
        }

        public bool CheckLeagueAccess(int userId, int organisationId)
        {
            var query = from o in _context.OrganisationList
                where o.OrganisationId == organisationId && o.UserId == userId
                select o;
            
            var data = query.ToList();
            if (data.Count == 0)
                return false;
            
            return true;
        }

        public IEnumerable<LeagueModel> GetLeaguesByOrganisation(int organisationId)
        {
            var query = from o in _context.Leagues
                where o.OrganisationId == organisationId
                select o;

            var data = query.ToList();

            return data;
        }
    }
}