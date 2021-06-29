using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Dtos.DoubleGoals;
using FoosballApi.Models.Goals;

namespace FoosballApi.Services
{
    public interface IFreehandDoubleGoalService
    {
        IEnumerable<FreehandDoubleGoalsJoinDto> GetAllFreehandGoals(int matchId, int userId);

    }
    public class FreehandDoubleGoalService : IFreehandDoubleGoalService
    {
        private readonly DataContext _context;

        public FreehandDoubleGoalService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<FreehandDoubleGoalsJoinDto> GetAllFreehandGoals(int matchId, int userId)
        {
            var query = (from fdg in _context.FreehandDoubleGoals
                from fdm in _context.FreehandDoubleMatches
                join u in _context.Users on fdg.ScoredByUserId equals u.Id
                where (fdg.ScoredByUserId == fdm.PlayerOneTeamA
                    || fdg.ScoredByUserId == fdm.PlayerOneTeamB || fdg.ScoredByUserId == fdm.PlayerTwoTeamA 
                    || fdg.ScoredByUserId == fdm.PlayerTwoTeamB) && fdg.DoubleMatchId.Equals(matchId)
                    
                select new FreehandDoubleGoalsJoinDto
                {
                    Id = fdg.Id,
                    ScoredByUserId = fdg.ScoredByUserId,
                    DoubleMatchId = fdg.DoubleMatchId,
                    ScorerTeamScore = fdg.ScorerTeamScore,
                    OpponentTeamScore = fdg.OpponentTeamScore,
                    WinnerGoal = fdg.WinnerGoal,
                    TimeOfGoal = fdg.TimeOfGoal,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).Distinct().OrderBy(f => f.Id).ToList();

            return query;
        }
    }
}