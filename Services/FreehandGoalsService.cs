using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Models.Goals;
using System.Linq;
using FoosballApi.Dtos.Matches;
using System;
using FoosballApi.Models;
using FoosballApi.Models.Matches;

namespace FoosballApi.Services
{
    public interface IFreehandGoalsService
    {
        IEnumerable<FreehandGoalModel> GetFreehandGoalsByMatchId(int matchId, int userId);
    }
    public class FreehandGoalsService : IFreehandGoalsService
    {
        private readonly DataContext _context;

        public FreehandGoalsService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<FreehandGoalModel> GetFreehandGoalsByMatchId(int matchId, int userId)
        {
            var query = from lp in _context.FreehandGoals
                        where lp.MatchId == matchId
                        orderby lp.Id, lp.TimeOfGoal
                        select new FreehandGoalModel
                        {
                            Id = lp.Id,
                            TimeOfGoal = lp.TimeOfGoal,
                            MatchId = lp.MatchId,
                            ScoredByUserId = lp.scoredByUserId.Id,
                            OponentId = lp.oponentId.Id,
                            ScoredByScore = lp.ScoredByScore,
                            OponentScore = lp.OponentScore,
                            WinnerGoal = lp.WinnerGoal
                        };

            return query.ToList();
        }
    }
}
