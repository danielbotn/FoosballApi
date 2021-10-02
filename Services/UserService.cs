using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Models;
using FoosballApi.Models.Users;

namespace FoosballApi.Services
{
    public interface IUserService
    {
        bool SaveChanges();
        IEnumerable<User> GetAllUsers();
        User GetUserByEmail(string email);
        User GetUserById(int id);
        void UpdateUser(User user);
        void DeleteUser(User user);
        UserStats GetUserStats(int userId);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public void DeleteUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.Users.Remove(user);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserByEmail(string email)
        {
            var query = from u in _context.Users
                        where u.Email == email
                        select u;
            var user = query.FirstOrDefault<User>();
            return user;
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(p => p.Id == id);
        }

        public UserStats GetUserStats(int userId)
        {
            (int, int) freehandMatches = GetSingleFreehandMatchesWonAndLost(userId);
            (int, int) doubleFreehandMatches = GetDoubleFreehandMatchesWonAndLost(userId);
            (int, int) singleLeagueMatches = GetSingleLeagueMatchesWonAndLost(userId);
            (int, int) doubleLeagueMatches = GetDoubleLeagueMatchesWonAndLost(userId);

            (int, int) freeHandGoals = GetFreehandGoalsScoredAndReceived(userId);
            (int, int?) doubleFreehandGoals = GetDoubleFreehandGoalsScoredAndReceived(userId);
            (int, int) singleLeagueGoals = GetSingleLeagueGoalsScoredAndReceived(userId);
            (int?, int?) doubleLeagueGoals = GetDoubleLeagueGoalsScoredAndReceived(userId);
            
            UserStats userStats = new UserStats
            {
                UserId = userId,
                TotalMatches = GetTotalMatchesByPlayer(userId),
                TotalMatchesWon = freehandMatches.Item1 + doubleFreehandMatches.Item1 + singleLeagueMatches.Item1 + doubleLeagueMatches.Item1,
                TotalMatchesLost = freehandMatches.Item2 + doubleFreehandMatches.Item2 + singleLeagueMatches.Item2 + doubleLeagueMatches.Item2,
                TotalGoalsScored = freeHandGoals.Item1 + doubleFreehandGoals.Item1 + singleLeagueGoals.Item1 + (int)doubleLeagueGoals.Item1,
                TotalGoalsReceived = freeHandGoals.Item2 + (int)doubleFreehandGoals.Item2 + singleLeagueGoals.Item2 + (int)doubleLeagueGoals.Item2
            };
            
            return userStats;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateUser(User user)
        {
            // Do nothing
        }

        private int GetTotalSingleFreehandMatches(int userId)
        {
            return _context.FreehandMatches
                .Where(x => x.PlayerOneId == userId || x.PlayerTwoId == userId)
                .Select(x => x.Id)
                .Count();
        }

        private int GetTotalDoubleFreehandMatches(int userId)
        {
            int TotalMatches = _context.FreehandDoubleMatches
                .Where(
                    x => x.PlayerOneTeamA == userId
                    || x.PlayerOneTeamB == userId 
                    || x.PlayerTwoTeamA == userId 
                    || x.PlayerTwoTeamB == userId
                )
                .Select(x => x.Id)
                .Count();

            return TotalMatches;
        }

        private int GetTotalSingleLeagueMatches(int userId)
        {
            return _context.SingleLeagueMatches
                .Where(x => x.PlayerOne == userId || x.PlayerTwo == userId)
                .Select(x => x.Id)
                .Count();
        }

        private int GetTotalDoubleLeagueMatches(int userId)
        {
            var query = from dlm in _context.DoubleLeagueMatches
                        join dlp in _context.DoubleLeaguePlayers
                        on new{PropertyName1 = dlm.TeamOneId, PropertyName2 = dlm.TeamTwoId} 
                        equals new{PropertyName1 = dlp.DoubleLeagueTeamId, PropertyName2 = dlp.DoubleLeagueTeamId}
                        where dlp.UserId == userId
                        select dlm.Id;

            return query.Count();
        }

        private int GetTotalMatchesByPlayer(int userId)
        {
            int result = 
                GetTotalDoubleLeagueMatches(userId) 
                + GetTotalSingleLeagueMatches(userId) 
                + GetTotalDoubleFreehandMatches(userId) 
                + GetTotalSingleFreehandMatches(userId);

            return result;
        }

        private (int, int) GetSingleFreehandMatchesWonAndLost(int userId)
        {
            (int, int) result = (0, 0);
            int totalSingleFreehandMatches = GetTotalSingleFreehandMatches(userId);

            int totalMatchesWonAsPlayerOne = _context.FreehandMatches
                .Where(x => x.PlayerOneId == userId && x.PlayerOneScore > x.PlayerTwoScore)
                .Select(x => x.Id)
                .Count();
            
             int totalMatchesWonAsPlayerTwo = _context.FreehandMatches
                .Where(x => x.PlayerTwoId == userId && x.PlayerTwoScore > x.PlayerOneScore)
                .Select(x => x.Id)
                .Count();
            
            int totalMatchesWon = totalMatchesWonAsPlayerOne + totalMatchesWonAsPlayerTwo;

            result.Item1 = totalMatchesWon;
            result.Item2 = totalSingleFreehandMatches - totalMatchesWon;

            return result;
        }

        private (int, int) GetDoubleFreehandMatchesWonAndLost(int userId)
        {
            (int, int) result = (0, 0);
            int totalDoubleFreehandMatches = GetTotalDoubleFreehandMatches(userId);

            int totalMatchesWonAsTeamA = _context.FreehandDoubleMatches
                .Where(x => (x.PlayerOneTeamA == userId || x.PlayerTwoTeamA == userId) && x.TeamAScore > x.TeamBScore)
                .Select(x => x.Id)
                .Count();
            
            int totalMatchesWonAsTeamB = _context.FreehandDoubleMatches
                .Where(x => (x.PlayerOneTeamB == userId || x.PlayerTwoTeamB == userId) && x.TeamBScore > x.TeamAScore)
                .Select(x => x.Id)
                .Count();

            int totalMatchesWon = totalMatchesWonAsTeamA + totalMatchesWonAsTeamB;
            result.Item1 = totalMatchesWon;
            result.Item2 = totalDoubleFreehandMatches - totalMatchesWon;

            return result;
        }

        private (int, int) GetSingleLeagueMatchesWonAndLost(int userId)
        {
            (int, int) result = (0, 0);
            int totalSingleLeagueMatches = GetTotalSingleLeagueMatches(userId);

            int totalMatchesWonAsPlayerOne = 
                _context.SingleLeagueMatches
                .Where(x => x.PlayerOne == userId && x.PlayerOneScore > x.PlayerTwoScore)
                .Select(x => x.Id)
                .Count();

            int totalMatchesWonAsPlayerTwo = 
                _context.SingleLeagueMatches
                .Where(x => x.PlayerTwo == userId && x.PlayerTwoScore > x.PlayerOneScore)
                .Select(x => x.Id)
                .Count();

            int totalMatchesWon = totalMatchesWonAsPlayerOne + totalMatchesWonAsPlayerTwo;

            result.Item1 = totalMatchesWon;
            result.Item2 = totalSingleLeagueMatches - totalMatchesWon;

            return result;
        }

        private (int, int) GetDoubleLeagueMatchesWonAndLost(int userId)
        {
            (int, int) result = (0, 0);
            int totalMatches = GetTotalDoubleLeagueMatches(userId);

            var matchesWonAsTeamOne = from dlm in _context.DoubleLeagueMatches
                        join dlp in _context.DoubleLeaguePlayers on dlm.TeamOneId equals dlp.DoubleLeagueTeamId
                        where dlp.UserId == userId && dlm.TeamOneScore > dlm.TeamTwoScore
                        select dlm.Id;
            
            var matchesWonAsTeamTwo = from dlm in _context.DoubleLeagueMatches
                        join dlp in _context.DoubleLeaguePlayers on dlm.TeamTwoId equals dlp.DoubleLeagueTeamId
                        where dlp.UserId == userId && dlm.TeamTwoScore > dlm.TeamOneScore
                        select dlm.Id;
            
            int totalMatchesWon = matchesWonAsTeamOne.Count() + matchesWonAsTeamTwo.Count();

            result.Item1 = totalMatchesWon;
            result.Item2 = totalMatches - totalMatchesWon;

            return result;
        }

        private (int, int) GetFreehandGoalsScoredAndReceived(int userId)
        {
            (int, int) result = (0, 0);

            int totalGoalsScored = _context.FreehandGoals.Where(x => x.ScoredByUserId == userId).Select(x => x.Id).Count();
            int totalGoalsReceived = _context.FreehandGoals.Where(x => x.OponentId == userId).Select(x => x.Id).Count();

            result.Item1 = totalGoalsScored;
            result.Item2 = totalGoalsReceived;

            return result;
        }

        private (int, int?) GetDoubleFreehandGoalsScoredAndReceived(int userId)
        {
            (int, int?) result = (0, 0);

            int totalGoalsScored = _context.FreehandDoubleGoals.Where(x => x.ScoredByUserId == userId).Select(x => x.Id).Count();
            int? totalGoalsReceivedAsTeamA = _context.FreehandDoubleMatches
                .Where(x => x.PlayerOneTeamA == userId || x.PlayerTwoTeamA == userId)
                .Select(x => x.TeamBScore)
                .Sum();
            int? totalGoalsReceivedAsTeamB = _context.FreehandDoubleMatches
                .Where(x => x.PlayerOneTeamB == userId || x.PlayerTwoTeamB == userId)
                .Select(x => x.TeamAScore)
                .Sum();
            
            result.Item1 = totalGoalsScored;
            result.Item2 = totalGoalsReceivedAsTeamA + totalGoalsReceivedAsTeamB;
            return result;
        }

        private (int, int) GetSingleLeagueGoalsScoredAndReceived(int userId)
        {
            (int, int) result = (0, 0);

            int totalGoalsScored = _context.SingleLeagueGoals.Where(x => x.ScoredByUserId == userId).Select(x => x.Id).Count();
            int totalGoalsReceived = _context.SingleLeagueGoals.Where(x => x.OpponentId == userId).Select(x => x.Id).Count();

            result.Item1 = totalGoalsScored;
            result.Item2 = totalGoalsReceived;

            return result;
        }

        private (int?, int?) GetDoubleLeagueGoalsScoredAndReceived(int userId)
        {
            (int?, int?) result = (0, 0);

            var totalGoalsScoredAsTeamOne = from dlm in _context.DoubleLeagueMatches
                join dlp in _context.DoubleLeaguePlayers on dlm.TeamOneId equals dlp.DoubleLeagueTeamId
                where dlp.UserId == userId
                select dlm.TeamOneScore;
            
            var totalGoalsScoredAsTeamTwo = from dlm in _context.DoubleLeagueMatches
                join dlp in _context.DoubleLeaguePlayers on dlm.TeamTwoId equals dlp.DoubleLeagueTeamId
                where dlp.UserId == userId
                select dlm.TeamTwoScore;

            var totalGoalsReceivedAsTeamOne = from dlm in _context.DoubleLeagueMatches
                join dlp in _context.DoubleLeaguePlayers on dlm.TeamOneId equals dlp.DoubleLeagueTeamId
                where dlp.UserId == userId
                select dlm.TeamTwoScore;
            
            var totalGoalsReceivedAsTeamTwo = from dlm in _context.DoubleLeagueMatches
                join dlp in _context.DoubleLeaguePlayers on dlm.TeamTwoId equals dlp.DoubleLeagueTeamId
                where dlp.UserId == userId
                select dlm.TeamOneScore;
            
            int? totalGoalsAsTeamOne = totalGoalsScoredAsTeamOne.Sum();
            int? totalGoalsAsTeamTwo = totalGoalsScoredAsTeamTwo.Sum();
            int? totalReceivedGoalsAsTeamOne = totalGoalsReceivedAsTeamOne.Sum();
            int? totalReceivedGoalsAsTeamTwo = totalGoalsReceivedAsTeamTwo.Sum();

            result.Item1 = totalGoalsAsTeamOne + totalGoalsAsTeamTwo;
            result.Item2 = totalReceivedGoalsAsTeamOne + totalReceivedGoalsAsTeamTwo;
            return result;
        }
    }
}