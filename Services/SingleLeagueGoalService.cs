using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Dtos.SingleLeagueGoals;
using FoosballApi.Models.Matches;
using FoosballApi.Models.SingleLeagueGoals;

namespace FoosballApi.Services
{
    public interface ISingleLeagueGoalService
    {
        IEnumerable<SingleLeagueGoalModel> GetAllSingleLeagueGoalsByMatchId(int matchId);
        bool CheckSingleLeagueGoalPermission(int userId, int goalId, int organisationId);
        bool CheckCreatePermission(int userId, SingleLeagueCreateModel singleLeagueCreateModel);
        SingleLeagueGoalModel GetSingleLeagueGoalById(int goaldId);
        void DeleteSingleLeagueGoal(SingleLeagueGoalModel singleLeagueGoalModel);
        SingleLeagueGoalModel CreateSingleLeagueGoal(SingleLeagueCreateModel singleLeagueCreateMode);
    }

    public class SingleLeagueGoalService : ISingleLeagueGoalService
    {
        private readonly DataContext _context;

        public SingleLeagueGoalService(DataContext context)
        {
            _context = context;
        }

        public bool CheckCreatePermission(int userId, SingleLeagueCreateModel singleLeagueCreateModel)
        {
            bool result = false;

            if (userId == singleLeagueCreateModel.ScoredByUserId || userId == singleLeagueCreateModel.OpponentId)
                result = true;
            
            return result;
        }

        public bool CheckSingleLeagueGoalPermission(int userId, int goalId, int organisationId)
        {
            bool result = false;

            var goalQuery = _context.SingleLeagueGoals.Where(x => x.Id == goalId).FirstOrDefault();

            int matchId = goalQuery.MatchId;

            var matchQuery = _context.SingleLeagueMatches.Where(x => x.Id == matchId).FirstOrDefault();

            int leaguId = matchQuery.LeagueId;

            var leaguePlayersQuery = _context.LeaguePlayers.Where(x => x.UserId == userId && x.LeagueId == leaguId).ToList();

            foreach (var lp in leaguePlayersQuery)
            {
                if (lp.UserId == userId)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public SingleLeagueGoalModel CreateSingleLeagueGoal(SingleLeagueCreateModel singleLeagueCreateMode)
        {
            DateTime now = DateTime.Now;
            if (singleLeagueCreateMode == null)
            {
                throw new ArgumentNullException(nameof(singleLeagueCreateMode));
            }

            SingleLeagueGoalModel newGoal = new();
            newGoal.TimeOfGoal = now;
            newGoal.MatchId = singleLeagueCreateMode.MatchId;
            newGoal.ScoredByUserId = singleLeagueCreateMode.ScoredByUserId;
            newGoal.OpponentId = singleLeagueCreateMode.OpponentId;
            newGoal.ScorerScore = singleLeagueCreateMode.ScorerScore;
            newGoal.OpponentScore = singleLeagueCreateMode.OpponentScore;
            newGoal.WinnerGoal = singleLeagueCreateMode.WinnerGoal;
            
            _context.SingleLeagueGoals.Add(newGoal);
            _context.SaveChanges();

            return newGoal;
        }

        public void DeleteSingleLeagueGoal(SingleLeagueGoalModel singleLeagueGoalModel)
        {
            if (singleLeagueGoalModel == null)
            {
                throw new ArgumentNullException(nameof(singleLeagueGoalModel));
            }

            var matchToChange = _context.SingleLeagueMatches.Where(x => x.Id == singleLeagueGoalModel.MatchId).FirstOrDefault();

            _context.SingleLeagueGoals.Remove(singleLeagueGoalModel);
            UpdateSingleLeagueMatchScore(matchToChange, singleLeagueGoalModel);
            _context.SaveChanges();
        }

        public IEnumerable<SingleLeagueGoalModel> GetAllSingleLeagueGoalsByMatchId(int matchId)
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

        public SingleLeagueGoalModel GetSingleLeagueGoalById(int goaldId)
        {
            return _context.SingleLeagueGoals.FirstOrDefault(x => x.Id == goaldId);
        }

        private void UpdateSingleLeagueMatchScore(SingleLeagueMatchModel matchToChange, SingleLeagueGoalModel singleLeagueGoalModel)
        {
            if (matchToChange.PlayerOne == singleLeagueGoalModel.ScoredByUserId)
            {
                if (matchToChange.PlayerOneScore > 0)
                    matchToChange.PlayerOneScore -= 1;
            }
            if (matchToChange.PlayerTwo == singleLeagueGoalModel.ScoredByUserId)
            {
                if (matchToChange.PlayerTwoScore > 0)
                    matchToChange.PlayerTwoScore -= 1;
            }
            _context.SingleLeagueMatches.Update(matchToChange);
        }
    }
}