using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Models.Goals;
using System.Linq;
using FoosballApi.Dtos.Matches;
using System;
using FoosballApi.Models;
using FoosballApi.Models.Matches;
using FoosballApi.Dtos.Goals;

namespace FoosballApi.Services
{
    public interface IFreehandGoalsService
    {
        IEnumerable<FreehandGoalModel> GetFreehandGoalsByMatchId(int matchId, int userId);

        FreehandGoalModel CreateFreehandGoal(int userId, FreehandGoalCreateDto freehandMatchCreateDto);

        FreehandGoalModel GetFreehandGoalById(int goalId);

        void DeleteFreehandGoal(FreehandGoalModel freehandGoalModel);

        void UpdateFreehandGoal(FreehandGoalModel freehandMatchModel);

        bool SaveChanges();
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
                            ScoredByUserId = lp.ScoredByUserId,
                            OponentId = lp.OponentId,
                            ScoredByScore = lp.ScoredByScore,
                            OponentScore = lp.OponentScore,
                            WinnerGoal = lp.WinnerGoal
                        };

            return query.ToList();
        }

        public FreehandGoalModel CreateFreehandGoal(int userId, FreehandGoalCreateDto freehandGoalCreateDto)
        {
            FreehandGoalModel fhg = new FreehandGoalModel();
            DateTime now = DateTime.Now;
            fhg.MatchId = freehandGoalCreateDto.MatchId;
            fhg.OponentScore = freehandGoalCreateDto.OponentScore;
            fhg.ScoredByScore = freehandGoalCreateDto.ScoredByScore;
            fhg.ScoredByUserId = freehandGoalCreateDto.ScoredByUserId;
            fhg.OponentId = freehandGoalCreateDto.OponentId;
            fhg.TimeOfGoal = now;
            fhg.WinnerGoal = freehandGoalCreateDto.WinnerGoal;
            _context.FreehandGoals.Add(fhg);
            _context.SaveChanges();

            return fhg;
        }

        public FreehandGoalModel GetFreehandGoalById(int goalId)
        {
            return _context.FreehandGoals.FirstOrDefault(x => x.Id == goalId);
        }

        public void DeleteFreehandGoal(FreehandGoalModel freehandGoalModel)
        {
            if (freehandGoalModel == null)
            {
                throw new ArgumentNullException(nameof(freehandGoalModel));
            }
            _context.FreehandGoals.Remove(freehandGoalModel);
            _context.SaveChanges();
        }

        public void UpdateFreehandGoal(FreehandGoalModel freehandMatchModel)
        {
            // Do nothing
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
