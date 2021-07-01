using System;
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

        FreehandDoubleGoalModel GetFreehandDoubleGoal(int goalId);

        bool CheckGoalPermission(int userId, int matchId, int goalId);

        FreehandDoubleGoalModel CreateDoubleFreehandGoal(int userId, FreehandDoubleGoalCreateDto freehandDoubleGoalCreateDto);

        void DeleteFreehandGoal(FreehandDoubleGoalModel goalItem);

    }
    public class FreehandDoubleGoalService : IFreehandDoubleGoalService
    {
        private readonly DataContext _context;

        public FreehandDoubleGoalService(DataContext context)
        {
            _context = context;
        }

        public bool CheckGoalPermission(int userId, int matchId, int goalId)
        {
            var query = from fdg in _context.FreehandDoubleGoals
                        join fdm in _context.FreehandDoubleMatches on fdg.DoubleMatchId equals fdm.Id
                        where fdg.DoubleMatchId == matchId && fdg.Id == goalId
                        select new
                        {
                            DoubleMatchId = fdg.DoubleMatchId,
                            ScoredByUserId = fdg.ScoredByUserId,
                            PlayerOneTeamA = fdm.PlayerOneTeamA,
                            PlayerTwoTeamA = fdm.PlayerTwoTeamA,
                            PlayerOneTeamB = fdm.PlayerTwoTeamA,
                            PlayerTwoTeamB = fdm.PlayerTwoTeamB
                        };

            var data = query.FirstOrDefault();

            if (data.DoubleMatchId == matchId &&
                (userId == data.PlayerOneTeamA || userId == data.PlayerOneTeamB
                || userId == data.PlayerTwoTeamA || userId == data.PlayerTwoTeamB))
                return true;

            return false;
        }

        public FreehandDoubleGoalModel CreateDoubleFreehandGoal(int userId, FreehandDoubleGoalCreateDto freehandDoubleGoalCreateDto)
        {
            FreehandDoubleGoalModel fhg = new FreehandDoubleGoalModel();
            DateTime now = DateTime.Now;
            fhg.DoubleMatchId = freehandDoubleGoalCreateDto.DoubleMatchId;
            fhg.OpponentTeamScore = freehandDoubleGoalCreateDto.OpponentTeamScore;
            fhg.ScoredByUserId = freehandDoubleGoalCreateDto.ScoredByUserId;
            fhg.ScorerTeamScore = freehandDoubleGoalCreateDto.ScorerTeamScore;
            fhg.TimeOfGoal = now;
            fhg.WinnerGoal = freehandDoubleGoalCreateDto.WinnerGoal;
            _context.FreehandDoubleGoals.Add(fhg);
            _context.SaveChanges();

            return fhg;
        }

        public void DeleteFreehandGoal(FreehandDoubleGoalModel freehandDoubleGoalModel)
        {
            if (freehandDoubleGoalModel == null)
            {
                throw new ArgumentNullException(nameof(freehandDoubleGoalModel));
            }
            _context.FreehandDoubleGoals.Remove(freehandDoubleGoalModel);
            _context.SaveChanges();
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

        public FreehandDoubleGoalModel GetFreehandDoubleGoal(int goalId)
        {
            return _context.FreehandDoubleGoals.FirstOrDefault(x => x.Id == goalId);
        }
    }
}