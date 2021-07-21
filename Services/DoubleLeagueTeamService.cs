using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Models.DoubleLeagueTeams;

namespace FoosballApi.Services
{
    public interface IDoubleLeagueTeamService
    {
        IEnumerable<DoubleLeagueTeamModel> GetDoubleLeagueTeamsByLeagueId(int leagueId);

        bool CheckLeaguePermission(int leagueId, int userId);

        DoubleLeagueTeamModel CreateDoubleLeagueTeam(int leagueId, int currentOrganisationId, string name);

        bool CheckDoubleLeagueTeamPermission(int teamId, int userId, int currentOrganisationId);

        DoubleLeagueTeamModel GetDoubleLeagueTeamById(int teamId);
        
        void DeleteDoubleLeagueTeam(int teamId);
    }

    public class DoubleLeagueTeamService : IDoubleLeagueTeamService
    {
        private readonly DataContext _context;
        
        public DoubleLeagueTeamService(DataContext context)
        {
            _context = context;
        }

        public bool CheckDoubleLeagueTeamPermission(int teamId, int userId, int currentOrganisationId)
        {
            var query = _context.DoubleLeagueTeams.FirstOrDefault(x => x.Id == teamId);

            if (query.OrganisationId != currentOrganisationId)
                return false;

            var query2 = _context.OrganisationList.Where(x => x.OrganisationId == query.OrganisationId && x.UserId == userId).FirstOrDefault();

            if (query2.OrganisationId == currentOrganisationId && query2.UserId == userId)
                return true;

            
            return false;
        }

        public bool CheckLeaguePermission(int leagueId, int userId)
        {
            bool result = false;

            var query = from dlp in _context.DoubleLeaguePlayers
                        join dlt in _context.DoubleLeagueTeams on dlp.DoubleLeagueTeamId equals dlt.Id
                        where dlt.LeagueId == leagueId
                        select new DoubleLeagueTeamsJoinModel
                        {
                            Id = dlp.Id,
                            UserId = dlp.UserId,
                            DoubleLeagueTeamId = dlp.DoubleLeagueTeamId,
                            LeagueId = dlt.LeagueId
                        };

            var data = query.ToList();

            foreach (var item in data)
            {
                if (item.UserId == userId)
                {
                    result = true;
                    break;
                }
            }
            
            return result;
        }

        public DoubleLeagueTeamModel CreateDoubleLeagueTeam(int leagueId, int currentOrganisationId, string name)
        {
            DateTime now = DateTime.Now;
            DoubleLeagueTeamModel newTeam = new();
            newTeam.Name = name;
            newTeam.CreatedAt = now;
            newTeam.OrganisationId = currentOrganisationId;
            newTeam.LeagueId = leagueId;

            _context.DoubleLeagueTeams.Add(newTeam);
            _context.SaveChanges();

            return newTeam;
        }

        public void DeleteDoubleLeagueTeam(int teamId)
        {
            var itemToDelete = _context.DoubleLeagueTeams.FirstOrDefault(x => x.Id == teamId);

            if (itemToDelete == null)
            {
                throw new ArgumentNullException(nameof(itemToDelete));
            }

            _context.DoubleLeagueTeams.Remove(itemToDelete);
            _context.SaveChanges();
        }

        public DoubleLeagueTeamModel GetDoubleLeagueTeamById(int teamId)
        {
            return _context.DoubleLeagueTeams.FirstOrDefault(x => x.Id == teamId);
        }

        public IEnumerable<DoubleLeagueTeamModel> GetDoubleLeagueTeamsByLeagueId(int leagueId)
        {
            return _context.DoubleLeagueTeams.Where(x => x.LeagueId == leagueId).ToList();
        }
    }
}