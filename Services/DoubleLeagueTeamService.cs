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
    }

    public class DoubleLeagueTeamService : IDoubleLeagueTeamService
    {
        private readonly DataContext _context;
        
        public DoubleLeagueTeamService(DataContext context)
        {
            _context = context;
        }

        /* 
        select dlp.id, dlp.user_id, dlp.double_league_team_id, dlt.league_id from double_league_players dlp
        join double_league_teams dlt on dlp.double_league_team_id = dlt.id
        */
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

        public IEnumerable<DoubleLeagueTeamModel> GetDoubleLeagueTeamsByLeagueId(int leagueId)
        {
            return _context.DoubleLeagueTeams.Where(x => x.LeagueId == leagueId).ToList();
        }
    }
}