using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Models.Leagues;
using System.Linq;
using System;

namespace FoosballApi.Services
{
    public interface ILeagueService
    {
        IEnumerable<LeagueModel> GetLeaguesByOrganisation(int organisationId);

        bool CheckLeagueAccess(int userId, int organisationId);

        int GetOrganisationId(int leagueId);

        IEnumerable<LeaguePlayersJoinModel> GetLeaguesPlayers(int leagueId);

        void CreateLeague(LeagueModel leagueModel);
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

        public void CreateLeague(LeagueModel leagueModel)
        {
            if (leagueModel == null)
            {
                throw new ArgumentNullException(nameof(leagueModel));
            }
            _context.Leagues.Add(leagueModel);
            _context.SaveChanges();
        }

        public IEnumerable<LeagueModel> GetLeaguesByOrganisation(int organisationId)
        {
            var query = from o in _context.Leagues
                        where o.OrganisationId == organisationId
                        select o;

            var data = query.ToList();

            return data;
        }
        public IEnumerable<LeaguePlayersJoinModel> GetLeaguesPlayers(int leagueId)
        {
            var query = from lp in _context.LeaguePlayers
                        where lp.LeagueId == leagueId
                        join u in _context.Users on lp.UserId equals u.Id
                        select new LeaguePlayersJoinModel
                        {
                            Id = lp.Id,
                            UserId = lp.UserId,
                            LeagueId = lp.LeagueId,
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                        };

            var data = query.ToList();

            return data;
        }

        public int GetOrganisationId(int leagueId)
        {
            var query = from o in _context.Leagues
                        where o.Id == leagueId
                        select o.OrganisationId;

            var id = query.FirstOrDefault();
            return id;
        }
    }
}
