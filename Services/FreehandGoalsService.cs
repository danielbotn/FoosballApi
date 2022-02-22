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
        IEnumerable<FreehandGoalModelExtended> GetFreehandGoalsByMatchId(int matchId, int userId);
        FreehandGoalModel CreateFreehandGoal(int userId, FreehandGoalCreateDto freehandMatchCreateDto);
        FreehandGoalModelExtended GetFreehandGoalById(int goalId);
        FreehandGoalModel GetFreehandGoalByIdFromDatabase(int goalId);
        void DeleteFreehandGoal(FreehandGoalModel freehandGoalModel);
        void UpdateFreehandGoal(FreehandGoalModel freehandMatchModel);
        bool SaveChanges();
        bool CheckGoalPermission(int userId, int matchId, int goalId);
    }

    public class FreehandGoalsService : IFreehandGoalsService
    {
        private readonly DataContext _context;

        public FreehandGoalsService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<FreehandGoalModelExtended> GetFreehandGoalsByMatchId(int matchId, int userId)
        {
            List<FreehandGoalModelExtended> result = new List<FreehandGoalModelExtended>();
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
            var data = query.ToList();

            foreach (var item in data)
            {
                FreehandGoalModelExtended fgme = new FreehandGoalModelExtended{
                    Id = item.Id,
                    TimeOfGoal = item.TimeOfGoal,
                    GoalTimeStopWatch = CalculateGoalTimeStopWatch(item.TimeOfGoal, item.MatchId),
                    MatchId = item.MatchId,
                    ScoredByUserId = item.ScoredByUserId,
                    ScoredByUserFirstName = _context.Users.Where(u => u.Id == item.ScoredByUserId).FirstOrDefault().FirstName,
                    ScoredByUserLastName = _context.Users.Where(u => u.Id == item.ScoredByUserId).FirstOrDefault().LastName,
                    ScoredByUserPhotoUrl = _context.Users.Where(u => u.Id == item.ScoredByUserId).FirstOrDefault().PhotoUrl,
                    OponentId = item.OponentId,
                    OponentFirstName = _context.Users.Where(u => u.Id == item.OponentId).FirstOrDefault().FirstName,
                    OponentLastName = _context.Users.Where(u => u.Id == item.OponentId).FirstOrDefault().LastName,
                    OponentPhotoUrl = _context.Users.Where(u => u.Id == item.OponentId).FirstOrDefault().PhotoUrl,
                    ScoredByScore = item.ScoredByScore,
                    OponentScore = item.OponentScore,
                    WinnerGoal = item.WinnerGoal
                };
                result.Add(fgme);
            }

            return result;
        }

        private string CalculateGoalTimeStopWatch(DateTime timeOfGoal, int matchId)
        {
            var match = _context.FreehandMatches.Where(m => m.Id == matchId).FirstOrDefault();
            DateTime? matchStarted = match.StartTime;
            if (matchStarted == null)
            {
                matchStarted = DateTime.Now;
            }
            TimeSpan timeSpan = matchStarted.Value - timeOfGoal;
            string result = timeSpan.ToString(@"hh\:mm\:ss");
            string sub = result.Substring(0, 2);
            // remove first two characters if they are "00:"
            if (sub == "00")
            {
                result = result.Substring(3);
            }
            return result;
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

            UpdateFreehandMatchScore(userId, freehandGoalCreateDto);

            return fhg;
        }

        public FreehandGoalModelExtended GetFreehandGoalById(int goalId)
        {
            var data = _context.FreehandGoals.FirstOrDefault(x => x.Id == goalId);
            
            FreehandGoalModelExtended result = new FreehandGoalModelExtended {
                Id = data.Id,
                TimeOfGoal = data.TimeOfGoal,
                MatchId = data.MatchId,
                ScoredByUserId = data.ScoredByUserId,
                ScoredByUserFirstName = _context.Users.Where(u => u.Id == data.ScoredByUserId).FirstOrDefault().FirstName,
                ScoredByUserLastName = _context.Users.Where(u => u.Id == data.ScoredByUserId).FirstOrDefault().LastName,
                ScoredByUserPhotoUrl = _context.Users.Where(u => u.Id == data.ScoredByUserId).FirstOrDefault().PhotoUrl,
                OponentId = data.OponentId,
                OponentFirstName = _context.Users.Where(u => u.Id == data.OponentId).FirstOrDefault().FirstName,
                OponentLastName = _context.Users.Where(u => u.Id == data.OponentId).FirstOrDefault().LastName,
                OponentPhotoUrl = _context.Users.Where(u => u.Id == data.OponentId).FirstOrDefault().PhotoUrl,
                ScoredByScore = data.ScoredByScore,
                OponentScore = data.OponentScore,
                WinnerGoal = data.WinnerGoal
            };
            
            return result;
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

        public bool CheckGoalPermission(int userId, int matchId, int goalId)
        {
            var query = from fg in _context.FreehandGoals
                        join fm in _context.FreehandMatches on fg.MatchId equals fm.Id
                        where fg.MatchId == matchId && fg.Id == goalId
                        select new
                        {
                            MatchId = fg.MatchId,
                            ScoredByUserId = fg.ScoredByUserId,
                            PlayerOneId = fm.PlayerOneId,
                            PlayerTwoId = fm.PlayerTwoId,
                        };

            var data = query.FirstOrDefault();

            if (data.MatchId == matchId && (userId == data.PlayerOneId || userId == data.PlayerTwoId))
                return true;

            return false;
        }

        private void UpdateFreehandMatchScore(int userId, FreehandGoalCreateDto freehandGoalCreateDto)
        {
            FreehandMatchModel fmm = _context.FreehandMatches.FirstOrDefault(f => f.Id == freehandGoalCreateDto.MatchId);
            if (fmm.PlayerOneId == freehandGoalCreateDto.ScoredByUserId)
            {
                fmm.PlayerOneScore = freehandGoalCreateDto.ScoredByScore;
            }
            else
            {
                fmm.PlayerTwoScore = freehandGoalCreateDto.ScoredByScore;
            }

            // Check if match is finished
            if (freehandGoalCreateDto.WinnerGoal == true)
            {
                fmm.EndTime = DateTime.Now;
                fmm.GameFinished = true;
            }
           
            _context.SaveChanges();
        }

        public FreehandGoalModel GetFreehandGoalByIdFromDatabase(int goalId)
        {
            return _context.FreehandGoals.FirstOrDefault(x => x.Id == goalId);
        }
    }
}
