using System;
using System.Collections.Generic;
using System.Linq;
using FoosballApi.Data;
using FoosballApi.Models.DoubleLeagueMatches;

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
                        select new LeaguePermissionJoinModel{
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

                var allTeams = new AllMatchesModel {
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

            var allTeams = new AllMatchesModel {
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