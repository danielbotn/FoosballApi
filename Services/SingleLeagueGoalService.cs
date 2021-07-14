using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Models.SingleLeagueGoals;

namespace FoosballApi.Services
{
    public interface ISingleLeagueGoalService
    {
        IEnumerable<SingleLeagueGoalModel> GetAllSingleLeagueGoalsByLeagueId(int matchId);
    }
    public class SingleLeagueGoalService : ISingleLeagueGoalService
    {
        private readonly DataContext _context;

        public SingleLeagueGoalService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<SingleLeagueGoalModel> GetAllSingleLeagueGoalsByLeagueId(int matchId)
        {
            var query = _context.SingleLeagueGoals
                .Where(x => x.MatchId == matchId)
                .Select(slgm => new SingleLeagueGoalModel
                    {
                        Id = slgm.Id,
                        TimeOfGoal = slgm.TimeOfGoal,
                        MatchId = slgm.MatchId,
                        ScoredByUserId = slgm.ScoredByUserId,
                        OpponentId = slgm.OpponentId,
                        ScorerScore = slgm.ScorerScore,
                        OpponentScore = slgm.OpponentScore,
                        WinnerGoal = slgm.WinnerGoal
                    });

            return query;
        }
    }
}