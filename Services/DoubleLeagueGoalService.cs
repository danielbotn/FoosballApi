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
        Task<IEnumerable<DoubleLeagueGoalDapper>> GetAllDoubleLeagueGoalsByMatchId(int matchId);
        Task<DoubleLeagueGoalDapper> GetDoubleLeagueGoalById(int goalId);
        bool CheckPermissionByGoalId(int goalId, int userId);
    }
    public class DoubleLeagueGoalService : IDoubleLeagueGoalService
    {
        private readonly DataContext _context;
        
        public DoubleLeagueGoalService(DataContext context)
        {
            _context = context;
        }

        public bool CheckPermissionByGoalId(int goalId, int userId)
        {
            bool result = false;
            List<int> teamIds = new List<int>();
            int matchId = _context.DoubleLeagueGoals.FirstOrDefault(x => x.Id == goalId).MatchId;
            var matchData = _context.DoubleLeagueMatches.FirstOrDefault(x => x.Id == matchId);
            int leagueId = matchData.LeagueId;

            teamIds.Add(matchData.TeamOneId);
            teamIds.Add(matchData.TeamTwoId);

            foreach (var item in teamIds)
            {
                var doubleLeaguePlayerData = _context.DoubleLeaguePlayers.Where(x => x.DoubleLeagueTeamId == item);

                foreach (var element in doubleLeaguePlayerData)
                {
                    if (element.UserId == userId)
                    {
                        result = true;
                        break;
                    }
                    
                }

            }
            
            return result;
        }

        public async Task<IEnumerable<DoubleLeagueGoalDapper>> GetAllDoubleLeagueGoalsByMatchId(int matchId)
        {
            CancellationToken ct = new();

            var tx = await _context.Database.BeginTransactionAsync();

            var dapperReadData = await _context.QueryAsync<DoubleLeagueGoalDapper>(ct, $@"
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

        public async Task<DoubleLeagueGoalDapper> GetDoubleLeagueGoalById(int goalId)
        {
            CancellationToken ct = new();

            var tx = await _context.Database.BeginTransactionAsync();

            var dapperReadData = await _context.QueryAsync<DoubleLeagueGoalDapper>(ct, $@"
                select distinct dlg.id, dlg.time_of_goal, dlg.scored_by_team_id, dlg.opponent_team_id, dlg.scorer_team_score, 
                dlg.opponent_team_score, dlg.winner_goal, dlg.user_scorer_id, dlp.double_league_team_id, u.first_name as scorer_first_name, 
                u.last_name as scorer_last_name
                from double_league_goals dlg
                join double_league_players dlp on dlp.double_league_team_id = dlg.scored_by_team_id
                join users u on u.id = dlg.user_scorer_id
                where dlg.id = {goalId}
                order by dlg.id");

            return dapperReadData.FirstOrDefault();
        }
    }
}