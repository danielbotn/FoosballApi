using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Dtos.DoubleGoals;
using FoosballApi.Models.Goals;
using FoosballApi.Models.Matches;

namespace FoosballApi.Services
{
    public interface IFreehandDoubleGoalService
    {
        IEnumerable<FreehandDoubleGoalsExtendedDto> GetAllFreehandGoals(int matchId, int userId);
        FreehandDoubleGoalModel GetFreehandDoubleGoal(int goalId);
        bool CheckGoalPermission(int userId, int matchId, int goalId);
        FreehandDoubleGoalModel CreateDoubleFreehandGoal(int userId, FreehandDoubleGoalCreateDto freehandDoubleGoalCreateDto);
        void DeleteFreehandGoal(FreehandDoubleGoalModel goalItem);
        void UpdateFreehanDoubledGoal(FreehandDoubleGoalModel goalItem);
        bool SaveChanges();
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

            UpdateFreehandDoubleMatchScore(userId, freehandDoubleGoalCreateDto);

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

        public IEnumerable<FreehandDoubleGoalsExtendedDto> GetAllFreehandGoals(int matchId, int userId)
        {
            List<FreehandDoubleGoalsExtendedDto> result = new List<FreehandDoubleGoalsExtendedDto>();
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
                             Email = u.Email,
                             PhotoUrl = u.PhotoUrl
                         }).Distinct().OrderBy(f => f.Id).ToList();
                        
            foreach (var item in query)
            {
                FreehandDoubleGoalsExtendedDto fdg = new FreehandDoubleGoalsExtendedDto{
                    Id = item.Id,
                    ScoredByUserId = item.ScoredByUserId,
                    DoubleMatchId = item.DoubleMatchId,
                    ScorerTeamScore = item.ScorerTeamScore,
                    OpponentTeamScore = item.OpponentTeamScore,
                    WinnerGoal = item.WinnerGoal,
                    TimeOfGoal = item.TimeOfGoal,
                    GoalTimeStopWatch = CalculateGoalTimeStopWatch(item.TimeOfGoal, item.DoubleMatchId),
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    PhotoUrl = item.PhotoUrl
                };
                result.Add(fdg);
            }

            return result;
        }

        public FreehandDoubleGoalModel GetFreehandDoubleGoal(int goalId)
        {
            return _context.FreehandDoubleGoals.FirstOrDefault(x => x.Id == goalId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateFreehanDoubledGoal(FreehandDoubleGoalModel goalItem)
        {
            // Do nothing
        }

        private void UpdateFreehandDoubleMatchScore(int userId, FreehandDoubleGoalCreateDto freehandGoalCreateDto)
        {
            FreehandDoubleMatchModel fmm = _context.FreehandDoubleMatches.FirstOrDefault(f => f.Id == freehandGoalCreateDto.DoubleMatchId);
            if (fmm.PlayerOneTeamA == freehandGoalCreateDto.ScoredByUserId || fmm.PlayerTwoTeamA == freehandGoalCreateDto.ScoredByUserId)
            {
                fmm.TeamAScore = freehandGoalCreateDto.ScorerTeamScore;
            }
            else
            {
                fmm.TeamBScore = freehandGoalCreateDto.ScorerTeamScore;
            }

            // Check if match is finished
            if (freehandGoalCreateDto.WinnerGoal == true)
            {
                fmm.EndTime = DateTime.Now;
                fmm.GameFinished = true;
            }
           
            _context.SaveChanges();
        }

        private string CalculateGoalTimeStopWatch(DateTime timeOfGoal, int matchId)
        {
            var match = _context.FreehandDoubleMatches.Where(m => m.Id == matchId).FirstOrDefault();
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
    }
}