using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FoosballApi.Data;
using FoosballApi.Models.DoubleLeagueGoals;

namespace FoosballApi.Services
{
    public interface IDoubleLeagueGoalService
    {
        Task<IEnumerable<DoubleLeagueGoalsDapper>> GetAllDoubleLeagueGoalsByMatchId(int matchId);
    }
    public class DoubleLeagueGoalService : IDoubleLeagueGoalService
    {
        private readonly DataContext _context;
        
        public DoubleLeagueGoalService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoubleLeagueGoalsDapper>> GetAllDoubleLeagueGoalsByMatchId(int matchId)
        {
            CancellationToken ct = new();

            var tx = await _context.Database.BeginTransactionAsync();

            var dapperReadData = await _context.QueryAsync<DoubleLeagueGoalsDapper>(ct, $@"
                select distinct dlg.id, dlg.time_of_goal, dlg.scored_by_team_id, dlg.opponent_team_id, dlg.scorer_team_score, 
                dlg.opponent_team_score, dlg.winner_goal, dlg.user_scorer_id, dlp.double_league_team_id, u.first_name as scorer_first_name, 
                u.last_name as scorer_last_name
                from double_league_goals dlg
                join double_league_players dlp on dlp.double_league_team_id = dlg.scored_by_team_id
                join users u on u.id = dlg.user_scorer_id
                where dlg.match_id = {matchId}
                order by dlg.id");

            return dapperReadData;
        }
    }
}