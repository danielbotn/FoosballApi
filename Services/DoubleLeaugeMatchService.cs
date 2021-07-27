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
    }
}