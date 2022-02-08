using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Enums;
using FoosballApi.Models;
using FoosballApi.Models.DoubleLeagueMatches;
using FoosballApi.Models.Matches;
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
        IEnumerable<UserLastTen> GetLastTenMatchesByUserId(int userId);
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
                TotalFreehandMatches = GetTotalSingleFreehandMatches(userId),
                TotalDoubleFreehandMatches = GetTotalDoubleFreehandMatches(userId),
                TotalSingleLeagueMatches = GetTotalSingleLeagueMatches(userId),
                TotalDoubleLeagueMatches = GetTotalDoubleLeagueMatches(userId),
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

        public IEnumerable<UserLastTen> GetLastTenMatchesByUserId(int userId)
        {
            List<UserLastTen> result = new List<UserLastTen>();

            var lastTenFreehandMatches = GetLastTenFreehandMatches(userId);
            var lastTenFreehandDoubleMatches = GetLastTenFreehandDoubleMatches(userId);
            var lastTenSingleLeagueMatches = GetLastTenSingleLeagueMatches(userId);
            var lastTenDoubleLeagueMatches = GetLastTenDoubleLeagueMatches(userId);

            foreach (var fm in lastTenFreehandMatches)
            {
                result.Add(fm);
            }

            foreach (var fdm in lastTenFreehandDoubleMatches)
            {
                result.Add(fdm);
            }

            foreach (var slm in lastTenSingleLeagueMatches)
            {
                result.Add(slm);
            }

            foreach (var dlm in lastTenDoubleLeagueMatches)
            {
                result.Add(dlm);
            }

            return FilterLastTen(result);
        }

        private IEnumerable<UserLastTen> FilterLastTen(IEnumerable<UserLastTen> lastTen)
        {
            return lastTen.OrderByDescending(x => x.DateOfGame).Take(10);
        }

        private IEnumerable<UserLastTen> GetLastTenFreehandMatches(int userId)
        {
            List<UserLastTen> result = new List<UserLastTen>();
            var freehandMatches = _context.FreehandMatches
                .Where(x => x.PlayerOneId == userId || x.PlayerTwoId == userId && x.EndTime != null)
                .OrderByDescending(x => x.EndTime)
                .Take(10)
                .ToList();

            foreach (var item in freehandMatches)
            {
                UserLastTen lastTenObject = new UserLastTen
                {
                    TypeOfMatch = ETypeOfMatch.FreehandMatch,
                    TypeOfMatchName = ETypeOfMatch.FreehandMatch.ToString(),
                    UserId = userId,
                    TeamMateId = null,
                    MatchId = item.Id,
                    OpponentId = item.PlayerOneId == userId ? item.PlayerTwoId : item.PlayerOneId,
                    OpponentTwoId = null,
                    OpponentOneFirstName = item.PlayerOneId == userId ?
                        _context.Users.Where(x => x.Id == item.PlayerTwoId).Select(x => x.FirstName).FirstOrDefault().ToString() :
                        _context.Users.Where(x => x.Id == item.PlayerOneId).Select(x => x.FirstName).FirstOrDefault().ToString(),
                    OpponentOneLastName = item.PlayerOneId == userId ?
                        _context.Users.Where(x => x.Id == item.PlayerTwoId).Select(x => x.LastName).FirstOrDefault().ToString() :
                        _context.Users.Where(x => x.Id == item.PlayerOneId).Select(x => x.LastName).FirstOrDefault().ToString(),
                    OpponentTwoFirstName = null,
                    OpponentTwoLastName = null,
                    UserScore = item.PlayerOneId == userId ? item.PlayerTwoScore : item.PlayerTwoScore,
                    OpponentUserOrTeamScore = item.PlayerOneId == userId ? item.PlayerTwoScore : item.PlayerOneScore,
                    DateOfGame = (DateTime)item.EndTime
                };
                result.Add(lastTenObject);
            }
            return result;
        }

        private IEnumerable<UserLastTen> GetLastTenFreehandDoubleMatches(int userId)
        {
            List<UserLastTen> result = new List<UserLastTen>();

            var lastTenDoubleFreehandMatches = _context.FreehandDoubleMatches
                .Where(x => (x.PlayerOneTeamA == userId || x.PlayerTwoTeamA == userId || x.PlayerOneTeamB == userId || x.PlayerTwoTeamB == userId)
                    && x.EndTime != null)
                .OrderByDescending(x => x.EndTime)
                .Take(10)
                .ToList();

            foreach (var item in lastTenDoubleFreehandMatches)
            {
                int theUserScore, theOpponentScore;
                string opponentOneFirstName, opponentOneLastName, opponentTwoFirstName, opponentTwoLastName;

                if (item.PlayerOneTeamA != userId && item.PlayerTwoTeamA != userId)
                {
                    opponentOneFirstName = GetOpponentOneFirstName(item, "teamB");
                    opponentOneLastName = GetOpponentOneLastName(item, "teamB");
                    opponentTwoFirstName = GetOpponentTwoFirstName(item, "teamB");
                    opponentTwoLastName = GetOpponentTwoLastName(item, "teamB");
                }
                else
                {
                    opponentOneFirstName = GetOpponentOneFirstName(item, "teamA");
                    opponentOneLastName = GetOpponentOneLastName(item, "teamA");

                    if (item.UserPlayerTwoTeamA != null)
                    {
                        var opponentTwoData = _context.Users.Where(x => x.Id == item.UserPlayerTwoTeamA.Id);
                        if (opponentTwoData != null)
                        {
                            opponentTwoFirstName = opponentTwoData.Select(x => x.FirstName).SingleOrDefault();
                            opponentTwoLastName = opponentTwoData.Select(x => x.LastName).SingleOrDefault();
                        }
                        else
                        {
                            opponentTwoFirstName = null;
                            opponentTwoLastName = null;
                        }
                    }
                    else
                    {
                        opponentTwoFirstName = null;
                        opponentTwoLastName = null;
                    }
                }

                if (item.PlayerOneTeamA == userId || item.PlayerTwoTeamA == userId)
                {
                    theUserScore = (int)item.TeamBScore;
                    theOpponentScore = (int)item.TeamAScore;
                }
                else
                {
                    theUserScore = (int)item.TeamAScore;
                    theOpponentScore = (int)item.TeamBScore;
                }
                // athuga
                UserLastTen lastTenObject = new UserLastTen
                {
                    TypeOfMatch = ETypeOfMatch.DoubleFreehandMatch,
                    TypeOfMatchName = ETypeOfMatch.DoubleFreehandMatch.ToString(),
                    MatchId = item.Id,
                    UserId = userId,
                    OpponentId = item.PlayerOneTeamA != userId && item.PlayerTwoTeamA != userId ? item.PlayerOneTeamB : item.PlayerOneTeamA,
                    OpponentTwoId = item.PlayerOneTeamA != userId && item.PlayerTwoTeamA != userId ? item.PlayerTwoTeamB : item.PlayerTwoTeamA,
                    TeamMateId = item.PlayerOneTeamA != userId && item.PlayerTwoTeamA != userId ? item.PlayerOneTeamB != userId ? item.PlayerTwoTeamB : item.PlayerOneTeamB : item.PlayerOneTeamA != userId ? item.PlayerTwoTeamA : item.PlayerOneTeamA,
                    OpponentOneFirstName = opponentOneFirstName,
                    OpponentOneLastName = opponentOneLastName,
                    OpponentTwoFirstName = opponentTwoFirstName,
                    OpponentTwoLastName = opponentTwoLastName,
                    UserScore = theUserScore,
                    OpponentUserOrTeamScore = theOpponentScore,
                    DateOfGame = (DateTime)item.EndTime
                };
                result.Add(lastTenObject);
            }
            return result;
        }

        private string GetOpponentOneFirstName(FreehandDoubleMatchModel item, string teamAorTeamB)
        {
            string result = null;
            if (teamAorTeamB == "teamA" && item.UserPlayerOneTeamA != null)
            {
                result = _context.Users
                    .Where(x => x.Id == item.UserPlayerOneTeamA.Id)
                    .Select(x => x.FirstName).SingleOrDefault();
            }
            else if (teamAorTeamB == "teamB" && item.UserPlayerOneTeamB != null)
            {
                result = _context.Users
                    .Where(x => x.Id == item.UserPlayerOneTeamB.Id)
                    .Select(x => x.FirstName).SingleOrDefault();
            }
            return result;
        }

        private string GetOpponentOneLastName(FreehandDoubleMatchModel item, string teamAorTeamB)
        {
            string result = null;
            if (teamAorTeamB == "teamA" && item.UserPlayerOneTeamA != null)
            {
                result = _context.Users
                        .Where(x => x.Id == item.UserPlayerOneTeamA.Id)
                        .Select(x => x.LastName).SingleOrDefault();
            }
            else if (teamAorTeamB == "teamB" && item.UserPlayerOneTeamB != null)
            {
                result = _context.Users
                    .Where(x => x.Id == item.UserPlayerOneTeamB.Id)
                    .Select(x => x.LastName).SingleOrDefault();
            }
            return result;
        }

        private string GetOpponentTwoFirstName(FreehandDoubleMatchModel item, string teamAorTeamB)
        {
            string result = null;
            if (teamAorTeamB == "teamA" && item.UserPlayerTwoTeamA != null)
            {
                result = _context.Users
                        .Where(x => x.Id == item.UserPlayerTwoTeamA.Id)
                        .Select(x => x.FirstName).SingleOrDefault();
            }
            else if (teamAorTeamB == "teamB" && item.UserPlayerTwoTeamB != null)
            {
                result = _context.Users
                        .Where(x => x.Id == item.UserPlayerTwoTeamB.Id)
                        .Select(x => x.FirstName).SingleOrDefault();
            }
            return result;
        }

        private string GetOpponentTwoLastName(FreehandDoubleMatchModel item, string teamAorTeamB)
        {
            string result = null;
            if (teamAorTeamB == "teamA" && item.UserPlayerTwoTeamA != null)
            {
                result = _context.Users
                        .Where(x => x.Id == item.UserPlayerTwoTeamA.Id)
                        .Select(x => x.FirstName).SingleOrDefault();
            }
            else if (teamAorTeamB == "teamB" && item.UserPlayerTwoTeamB != null)
            {
                result = _context.Users
                        .Where(x => x.Id == item.UserPlayerTwoTeamB.Id)
                        .Select(x => x.LastName).SingleOrDefault();
            }
            return result;
        }


        private IEnumerable<UserLastTen> GetLastTenSingleLeagueMatches(int userId)
        {
            List<UserLastTen> result = new List<UserLastTen>();

            var lastTenSingleLeagueMatches = _context.SingleLeagueMatches
                .Where(x => (x.PlayerOne == userId || x.PlayerTwo == userId) && x.MatchEnded == true)
                .OrderByDescending(x => x.EndTime)
                .Take(10)
                .ToList();

            foreach (var item in lastTenSingleLeagueMatches)
            {
                int playerOne, playerTwo, playerOneScore, playerTwoScore;

                playerOne = item.PlayerOne;
                playerTwo = item.PlayerTwo;
                playerOneScore = (int)item.PlayerOneScore;
                playerTwoScore = (int)item.PlayerTwoScore;
                UserLastTen lastTenObject = new UserLastTen
                {
                    TypeOfMatch = ETypeOfMatch.FreehandMatch,
                    TypeOfMatchName = ETypeOfMatch.FreehandMatch.ToString(),
                    UserId = userId,
                    TeamMateId = null,
                    MatchId = item.Id,
                    OpponentId = item.PlayerOne == userId ? item.PlayerTwo : item.PlayerOne,
                    OpponentTwoId = null,
                    OpponentOneFirstName = item.PlayerOne == userId ?
                        _context.Users.Where(x => x.Id == item.PlayerTwo).Select(x => x.FirstName).SingleOrDefault() :
                        _context.Users.Where(x => x.Id == item.PlayerOne).Select(x => x.FirstName).SingleOrDefault(),
                    OpponentOneLastName = item.PlayerOne == userId ?
                        _context.Users.Where(x => x.Id == item.PlayerTwo).Select(x => x.LastName).SingleOrDefault() :
                        _context.Users.Where(x => x.Id == item.PlayerOne).Select(x => x.LastName).SingleOrDefault(),
                    OpponentTwoFirstName = null,
                    OpponentTwoLastName = null,
                    UserScore = playerOne == userId ? playerOneScore : playerTwoScore,
                    OpponentUserOrTeamScore = playerOne == userId ? playerTwoScore : playerOneScore,
                    DateOfGame = (DateTime)item.EndTime
                };
                result.Add(lastTenObject);
            }
            return result;
        }

        private IEnumerable<UserLastTen> GetLastTenDoubleLeagueMatches(int userId)
        {
            List<DoubleLeagueMatchModel> doubleLeagueMatches = new();
            List<int> teamIds = new();
            List<UserLastTen> result = new List<UserLastTen>();

            // First find all team ids of user
            var teamIdsData = _context.DoubleLeaguePlayers.Where(x => x.UserId == userId).Select(x => x.DoubleLeagueTeamId).ToList();

            foreach (var item in teamIdsData)
            {
                teamIds.Add(item);

                var tmp = _context.DoubleLeagueMatches.Where(x => x.TeamOneId == item || x.TeamTwoId == item);
                foreach (var element in tmp)
                {
                    doubleLeagueMatches.Add(element);
                }
            }

            foreach (var item in doubleLeagueMatches)
            {
                int opponentId;
                int opponentTwoId;
                int teamMateId;
                int userScore;
                int opponentScore;
                if (teamIds.Contains(item.TeamOneId))
                {
                    var uId = _context.DoubleLeaguePlayers.Where(x => x.DoubleLeagueTeamId == item.TeamOneId).Select(x => x.UserId).FirstOrDefault();
                    teamMateId = uId;
                    var opponentData = _context.DoubleLeaguePlayers.Where(x => x.DoubleLeagueTeamId == item.TeamTwoId).OrderBy(x => x.Id).Select(x => x.UserId);

                    opponentId = opponentData.First();
                    opponentTwoId = opponentData.Last();
                    userScore = (int)item.TeamOneScore;
                    opponentScore = (int)item.TeamTwoScore;
                }
                else
                {
                    var uId = _context.DoubleLeaguePlayers.Where(x => x.DoubleLeagueTeamId == item.TeamTwoId).Select(x => x.UserId).FirstOrDefault();
                    var opponentData = _context.DoubleLeaguePlayers.Where(x => x.DoubleLeagueTeamId == item.TeamOneId).Select(x => x.UserId);
                    teamMateId = uId;

                    opponentId = opponentData.First();
                    opponentTwoId = opponentData.Last();
                    userScore = (int)item.TeamTwoScore;
                    opponentScore = (int)item.TeamOneScore;
                }

                UserLastTen lastTenObject = new UserLastTen
                {
                    TypeOfMatch = ETypeOfMatch.DoubleLeagueMatch,
                    TypeOfMatchName = ETypeOfMatch.DoubleLeagueMatch.ToString(),
                    UserId = userId,
                    TeamMateId = teamMateId,
                    MatchId = item.Id,
                    OpponentId = opponentId,
                    OpponentTwoId = null,
                    OpponentOneFirstName = _context.Users.Where(x => x.Id == opponentId).Select(x => x.FirstName).SingleOrDefault(),
                    OpponentOneLastName = _context.Users.Where(x => x.Id == opponentId).Select(x => x.LastName).SingleOrDefault(),
                    OpponentTwoFirstName = _context.Users.Where(x => x.Id == opponentTwoId).Select(x => x.FirstName).SingleOrDefault(),
                    OpponentTwoLastName = _context.Users.Where(x => x.Id == opponentTwoId).Select(x => x.LastName).SingleOrDefault(),
                    UserScore = userScore,
                    OpponentUserOrTeamScore = opponentScore,
                    DateOfGame = (DateTime)item.EndTime
                };
                result.Add(lastTenObject);
            }

            return result;
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
                        on new { PropertyName1 = dlm.TeamOneId, PropertyName2 = dlm.TeamTwoId }
                        equals new { PropertyName1 = dlp.DoubleLeagueTeamId, PropertyName2 = dlp.DoubleLeagueTeamId }
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