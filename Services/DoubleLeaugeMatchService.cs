using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FoosballApi.Data;
using FoosballApi.Models;
using FoosballApi.Models.DoubleLeagueMatches;
using FoosballApi.Models.Other;

namespace FoosballApi.Services
{
    public interface IDoubleLeaugeMatchService
    {
        bool CheckLeaguePermission(int leagueId, int userId);
        IEnumerable<AllMatchesModel> GetAllMatchesByOrganisationId(int currentOrganisationId, int leagueId);
        bool CheckMatchAccess(int matchId, int userId, int currentOrganisationId);
        AllMatchesModel GetDoubleLeagueMatchById(int matchId);
        DoubleLeagueMatchModel GetDoubleLeagueMatchByIdSimple(int matchId);
        void UpdateDoubleLeagueMatch(DoubleLeagueMatchModel match);
        bool SaveChanges();
        DoubleLeagueMatchModel ResetMatch(DoubleLeagueMatchModel doubleLeagueMatchModel, int matchId);
        IEnumerable<DoubleLeagueStandingsQuery> GetDoubleLeagueStandings(int leagueId);
    }

    public class DoubleLeaugeMatchService : IDoubleLeaugeMatchService
    {
        private readonly DataContext _context;

        public DoubleLeaugeMatchService(DataContext context)
        {
            _context = context;
        }

        public bool CheckLeaguePermission(int leagueId, int userId)
        {
            bool result = false;
            var query = from dlp in _context.DoubleLeaguePlayers
                        where dlp.UserId == userId
                        join dlt in _context.DoubleLeagueTeams on dlp.DoubleLeagueTeamId equals dlt.Id
                        select new LeaguePermissionJoinModel
                        {
                            Id = dlp.Id,
                            LeagueId = dlt.LeagueId,
                            UserId = dlp.UserId
                        };

            var data = query.ToList();

            foreach (var item in data)
            {
                if (item.LeagueId == leagueId && item.UserId == userId)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public bool CheckMatchAccess(int matchId, int userId, int currentOrganisationId)
        {
            bool result = false;
            var query = _context.DoubleLeagueMatches.SingleOrDefault(x => x.Id == matchId);

            var query2 = _context.DoubleLeagueTeams.Where(x => x.Id == query.TeamOneId || x.Id == query.TeamTwoId).ToList();

            foreach (var item in query2)
            {
                if (item.OrganisationId == currentOrganisationId)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public IEnumerable<AllMatchesModel> GetAllMatchesByOrganisationId(int currentOrganisationId, int leagueId)
        {
            var query = _context.DoubleLeagueMatches.Where(x => x.LeagueId == leagueId).ToList();

            List<AllMatchesModel> result = new List<AllMatchesModel>();

            foreach (var item in query)
            {
                var subquery = from dlp in _context.DoubleLeaguePlayers
                               where dlp.DoubleLeagueTeamId == item.TeamOneId
                               join u in _context.Users on dlp.UserId equals u.Id
                               select new TeamModel
                               {
                                   Id = dlp.Id,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Email = u.Email
                               };

                var teamOne = subquery.ToArray();

                var subquery2 = from dlp in _context.DoubleLeaguePlayers
                                where dlp.DoubleLeagueTeamId == item.TeamTwoId
                                join u in _context.Users on dlp.UserId equals u.Id
                                select new TeamModel
                                {
                                    Id = dlp.Id,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Email = u.Email
                                };

                var teamTwo = subquery2.ToArray();

                var allTeams = new AllMatchesModel
                {
                    Id = item.Id,
                    TeamOneId = item.TeamOneId,
                    TeamTwoId = item.TeamTwoId,
                    LeagueId = item.LeagueId,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    TeamOneScore = (int)item.TeamOneScore,
                    TeamTwoScore = (int)item.TeamTwoScore,
                    MatchStarted = (bool)item.MatchStarted,
                    MatchEnded = (bool)item.MatchEnded,
                    MatchPaused = (bool)item.MatchPaused,
                    TeamOne = teamOne,
                    TeamTwo = teamTwo
                };
                result.Add(allTeams);

            }
            return result;
        }

        public AllMatchesModel GetDoubleLeagueMatchById(int matchId)
        {
            var query = _context.DoubleLeagueMatches.FirstOrDefault(x => x.Id == matchId);

            var subquery = from dlp in _context.DoubleLeaguePlayers
                           where dlp.DoubleLeagueTeamId == query.TeamOneId
                           join u in _context.Users on dlp.UserId equals u.Id
                           select new TeamModel
                           {
                               Id = dlp.Id,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               Email = u.Email
                           };

            var teamOne = subquery.ToArray();

            var subquery2 = from dlp in _context.DoubleLeaguePlayers
                            where dlp.DoubleLeagueTeamId == query.TeamTwoId
                            join u in _context.Users on dlp.UserId equals u.Id
                            select new TeamModel
                            {
                                Id = dlp.Id,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Email = u.Email
                            };

            var teamTwo = subquery2.ToArray();

            var allTeams = new AllMatchesModel
            {
                Id = query.Id,
                TeamOneId = query.TeamOneId,
                TeamTwoId = query.TeamTwoId,
                LeagueId = query.LeagueId,
                StartTime = query.StartTime,
                EndTime = query.EndTime,
                TeamOneScore = (int)query.TeamOneScore,
                TeamTwoScore = (int)query.TeamTwoScore,
                MatchStarted = (bool)query.MatchStarted,
                MatchEnded = (bool)query.MatchEnded,
                MatchPaused = (bool)query.MatchPaused,
                TeamOne = teamOne,
                TeamTwo = teamTwo
            };

            return allTeams;
        }

        public DoubleLeagueMatchModel GetDoubleLeagueMatchByIdSimple(int matchId)
        {
            return _context.DoubleLeagueMatches.FirstOrDefault(x => x.Id == matchId);
        }

        public IEnumerable<DoubleLeagueStandingsQuery> GetDoubleLeagueStandings(int leagueId)
        {
            List<DoubleLeagueStandingsQuery> standings = new();
            const int Points = 3;
            const int Zero = 0;
            List<int> teamIds = GetAllTeamIds(leagueId);

            foreach (var teamId in teamIds)
            {
                var matchesWonAsTeamOne = _context.DoubleLeagueMatches.Where(x => x.TeamOneId == teamId && x.MatchEnded == true && x.TeamOneScore > x.TeamTwoScore);
                var matchesWonAsTeamTwo = _context.DoubleLeagueMatches.Where(x => x.TeamTwoId == teamId && x.MatchEnded == true && x.TeamTwoScore > x.TeamOneScore);

                var matchesLostAsTeamOne = _context.DoubleLeagueMatches.Where(x => x.TeamOneId == teamId && x.MatchEnded == true && x.TeamOneScore < x.TeamTwoScore);
                var matchesLostAsTeamTwo = _context.DoubleLeagueMatches.Where(x => x.TeamTwoId == teamId && x.MatchEnded == true && x.TeamTwoScore < x.TeamOneScore);

                int totalMatchesWon = matchesWonAsTeamOne.Count() + matchesWonAsTeamTwo.Count();
                int totalMatchesLost = matchesLostAsTeamOne.Count() + matchesLostAsTeamTwo.Count();

                DoubleLeagueStandingsQuery dls = new DoubleLeagueStandingsQuery
                {
                    TeamID = teamId,
                    LeagueId = leagueId,
                    TotalMatchesWon = totalMatchesWon,
                    TotalMatchesLost = totalMatchesLost,
                    TotalGoalsScored = GetTotalGoalsScored(teamId),
                    TotalGoalsRecieved = GetTolalGoalsRecieved(teamId),
                    PositionInLeague = Zero,
                    MatchesPlayed = Zero,
                    Points = Points * totalMatchesWon,
                    TeamMembers = GetTeamMembers(teamId).ToArray()
                };
                standings.Add(dls);
            }

            var sortedLeague = ReturnSortedLeague(standings);
            var sortedLeagueWithPositions = AddPositionInLeagueToList(sortedLeague);

            return sortedLeagueWithPositions;
        }

        public DoubleLeagueMatchModel ResetMatch(DoubleLeagueMatchModel doubleLeagueMatchModel, int matchId)
        {
            if (doubleLeagueMatchModel == null)
                throw new ArgumentNullException(nameof(doubleLeagueMatchModel));

            DeleteAllGoals(matchId);

            return ResetDoubleLeagueMatch(doubleLeagueMatchModel);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateDoubleLeagueMatch(DoubleLeagueMatchModel match)
        {
            // Do nothing
        }

        private int GetTotalGoalsScored(int teamId)
        {
            int result = 0;
            var query = _context.DoubleLeagueGoals.Where(x => x.ScoredByTeamId == teamId).Select(x => x.Id).ToList();

            result = query.Count();

            return result;
        }

        private int GetTolalGoalsRecieved(int teamId)
        {
            int result = 0;

            var query = _context.DoubleLeagueGoals.Where(x => x.OpponentTeamId == teamId).Select(x => x.Id).ToList();

            result = query.Count();

            return result;
        }

        private List<DoubleLeagueStandingsQuery> AddPositionInLeagueToList(List<DoubleLeagueStandingsQuery> standings)
        {
            List<DoubleLeagueStandingsQuery> result = standings;
            foreach (var item in result.Select((value, i) => new { i, value }))
            {
                item.value.PositionInLeague = item.i + 1;
            }
            return result;
        }

        private List<DoubleLeagueStandingsQuery> ReturnSortedLeague(List<DoubleLeagueStandingsQuery> singleLeagueStandings)
        {
            return singleLeagueStandings.OrderByDescending(x => x.Points).ToList();
        }

        private List<int> GetAllTeamIds(int leagueId)
        {
            List<int> result = new();

            var query = _context.DoubleLeagueMatches
                .Where(x => x.LeagueId == leagueId)
                .Select(m => new DoubleLeagueMatchesSelect
                {
                    TeamOneId = m.TeamOneId,
                    TeamTwoId = m.TeamTwoId
                })
                .Distinct()
                .ToList();

            foreach (var item in query)
            {
                result.Add(item.TeamOneId);
                result.Add(item.TeamTwoId);
            }
            return result;
        }

        private List<TeamMember> GetTeamMembers(int teamId)
        {
            List<TeamMember> teamMembers = new();

            var players = _context.DoubleLeaguePlayers
                .Where(x => x.DoubleLeagueTeamId == teamId)
                .Select(x => new { UserId = x.UserId })
                .Distinct()
                .ToList();

            foreach (var player in players)
            {
                var user = _context.Users
                    .Where(x => x.Id == player.UserId)
                    .Select(m => new User
                    {
                        Id = m.Id,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Email = m.Email
                    })
                    .FirstOrDefault();

                TeamMember teamMember = new TeamMember
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                teamMembers.Add(teamMember);
            }

            return teamMembers;
        }

        private void DeleteAllGoals(int matchId)
        {
            var allGoals = _context.DoubleLeagueGoals.Where(x => x.MatchId == matchId).ToList();

            _context.DoubleLeagueGoals.RemoveRange(allGoals);
            _context.SaveChanges();
        }

        private DoubleLeagueMatchModel ResetDoubleLeagueMatch(DoubleLeagueMatchModel doubleLeagueMatchModel)
        {
            doubleLeagueMatchModel.StartTime = null;
            doubleLeagueMatchModel.EndTime = null;
            doubleLeagueMatchModel.TeamOneScore = 0;
            doubleLeagueMatchModel.TeamTwoScore = 0;
            doubleLeagueMatchModel.MatchStarted = false;
            doubleLeagueMatchModel.MatchEnded = false;
            doubleLeagueMatchModel.MatchPaused = false;

            _context.DoubleLeagueMatches.Update(doubleLeagueMatchModel);
            _context.SaveChanges();

            return doubleLeagueMatchModel;
        }
    }
}