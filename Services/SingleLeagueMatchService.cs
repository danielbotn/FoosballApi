using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Dtos.SingleLeagueMatches;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FoosballApi.Models.Matches;
using FoosballApi.Models.Other;

namespace FoosballApi.Services
{
    public interface ISingleLeagueMatchService
    {
        IEnumerable<SingleLeagueMatchesQuery> GetAllMatchesByOrganisationId(int organisationId, int leagueId);

        bool CheckLeaguePermission(int leagueId, int userId);
    }
    public class SingleLeagueMatchService : ISingleLeagueMatchService
    {
        private readonly DataContext _context;

        public SingleLeagueMatchService(DataContext context)
        {
            _context = context;
        }

        public bool CheckLeaguePermission(int leagueId, int userId)
        {
           var query = _context.LeaguePlayers.Where(x => x.LeagueId == leagueId && x.UserId == userId);

            var data = query.FirstOrDefault();

            if (data.UserId == userId && data.LeagueId == leagueId)
                return true;

           return false;
        }

        public IEnumerable<SingleLeagueMatchesQuery> GetAllMatchesByOrganisationId(int organisationId, int leagueId)
        {
            var query = _context.Set<SingleLeagueMatchesQuery>().FromSqlRaw(
                "SELECT slm.id, slm.player_one, slm.player_two, slm.league_id, slm.start_time, slm.end_time, " + 
                "slm.player_one_score, slm.player_two_score, slm.match_ended, slm.match_paused, slm.match_started, " + 
                "l.organisation_id, " +
                "(SELECT u.first_name from Users u where u.id = slm.player_one) AS player_one_first_name, " +
                "(SELECT u2.last_name from Users u2 where u2.id = slm.player_one) AS player_one_last_name, " +
                "(SELECT u3.first_name from Users u3 where u3.id = slm.player_two) as player_two_first_name, " +
                "(SELECT u4.last_name from Users u4 where u4.id = slm.player_two) as player_two_last_name " +
                "FROM single_league_matches slm " +
                "JOIN leagues l on l.id = slm.league_id " +
                $"where league_id = {leagueId}"
                );

            var data = query.ToList();

            return data;

        }
    }
}