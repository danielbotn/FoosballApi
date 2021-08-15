using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Dtos.SingleLeagueMatches;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FoosballApi.Models.Matches;
using FoosballApi.Models.Other;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace FoosballApi.Services
{
    public interface ISingleLeagueMatchService
    {
        IEnumerable<SingleLeagueMatchesQuery> GetAllMatchesByOrganisationId(int organisationId, int leagueId);

        bool CheckLeaguePermission(int leagueId, int userId);

        bool CheckMatchPermission(int matchId, int userId);

        SingleLeagueMatchModel GetSingleLeagueMatchById(int matchId);

        void UpdateSingleLeagueMatch(SingleLeagueMatchModel match);

        bool SaveChanges();

        IEnumerable<SingleLeagueStandingsQuery> GetSigleLeagueStandings(int leagueId);

        Task<IEnumerable<SingleLeagueMathModelDapper>> TestDapper(CancellationToken ct);

        Task<SingleLeagueMatchModel> ResetMatch(SingleLeagueMatchModel singleLeagueMatchModel, int matchId);
    }
    public class SingleLeagueMatchService : ISingleLeagueMatchService
    {
        private readonly DataContext _context;

        public SingleLeagueMatchService(DataContext context)
        {
            _context = context;
        }

        public bool CheckLeaguePermission(int leagueId, int userId)
        {
            var query = _context.LeaguePlayers.Where(x => x.LeagueId == leagueId && x.UserId == userId);

            var data = query.FirstOrDefault();

            if (data.UserId == userId && data.LeagueId == leagueId)
                return true;

            return false;
        }

        public bool CheckMatchPermission(int matchId, int userId)
        {
            var query = _context.SingleLeagueMatches.Where(x => x.Id == matchId && x.PlayerOne == userId || x.PlayerOne == userId);

            var data = query.FirstOrDefault();

            if (data.PlayerOne == userId || data.PlayerTwo == userId)
                return true;

            return false;
        }

        public IEnumerable<SingleLeagueMatchesQuery> GetAllMatchesByOrganisationId(int organisationId, int leagueId)
        {
            var query = _context.Set<SingleLeagueMatchesQuery>().FromSqlRaw(
                "SELECT slm.id, slm.player_one, slm.player_two, slm.league_id, slm.start_time, slm.end_time, " +
                "slm.player_one_score, slm.player_two_score, slm.match_ended, slm.match_paused, slm.match_started, " +
                "l.organisation_id, " +
                "(SELECT u.first_name from Users u where u.id = slm.player_one) AS player_one_first_name, " +
                "(SELECT u2.last_name from Users u2 where u2.id = slm.player_one) AS player_one_last_name, " +
                "(SELECT u3.first_name from Users u3 where u3.id = slm.player_two) as player_two_first_name, " +
                "(SELECT u4.last_name from Users u4 where u4.id = slm.player_two) as player_two_last_name " +
                "FROM single_league_matches slm " +
                "JOIN leagues l on l.id = slm.league_id " +
                $"where league_id = {leagueId}"
                );

            return query.ToList();
        }

        public IEnumerable<SingleLeagueStandingsQuery> GetSigleLeagueStandings(int leagueId)
        {
            const int Points = 3;
            const int Zero = 0;
            List<int> userIds = GetAllUsersOfLeague(leagueId);
            List<SingleLeagueStandingsQuery> standings = new();

            foreach (int element in userIds)
            {
                var matchesWonAsPlayerOne = _context.Set<SingleLeagueStandingsMatchesWonAsPlayerOne>().FromSqlRaw(
                    "select count(*) as matches_won_as_player_one from " +
                    "(select slm.id from single_league_matches slm " +
                    $"where slm.player_one = {element} and slm.match_ended = true and slm.player_one_score > slm.player_two_score) t"
                );

                var matchesWonAsPlayerTwo = _context.Set<SingleLeagueStandingsMatchesWonAsPlayerTwo>().FromSqlRaw(
                    "select count(*) as matches_won_as_player_two from " +
                    "(select slm.id from single_league_matches slm " +
                    $"where slm.player_two = {element} and slm.match_ended = true and slm.player_two_score > slm.player_one_score) t"
                );

                var matchesLostAsPlayerOne = _context.Set<SingleLeagueStandingsMatchesLostAsPlayerOne>().FromSqlRaw(
                    "select count(*) as matches_lost_as_player_one from " +
                    "(select slm.id from single_league_matches slm " +
                    $"where slm.player_one = {element} and slm.match_ended = true and slm.player_one_score < slm.player_two_score) t"
                );

                var matchesLostAsPlayerTwo = _context.Set<SingleLeagueStandingsMatchesLostAsPlayerTwo>().FromSqlRaw(
                    "select count(*) as matches_lost_as_player_two from " +
                    "(select slm.id from single_league_matches slm " +
                    $"where slm.player_two = {element} and slm.match_ended = true and slm.player_two_score < slm.player_one_score) t"
                );

                var userInfo = _context.Users.Where(x => x.Id == element);

                int totalMatchesWon = matchesWonAsPlayerOne.FirstOrDefault().MatchesWonAsPlayerOne +
                matchesWonAsPlayerTwo.FirstOrDefault().MatchesWonAsPlayerTwo;

                int totalMatchesLost = matchesLostAsPlayerOne.FirstOrDefault().MatchesLostAsPlayerOne +
                matchesLostAsPlayerTwo.FirstOrDefault().MatchesLostAsPlayerTwo;

                standings.Add(
                    new SingleLeagueStandingsQuery(
                        element,
                        leagueId,
                        totalMatchesWon,
                        totalMatchesLost,
                        Zero,
                        Zero,
                        Zero,
                        (totalMatchesLost + totalMatchesWon),
                        totalMatchesWon * Points,
                        userInfo.FirstOrDefault().FirstName,
                        userInfo.FirstOrDefault().LastName,
                        userInfo.FirstOrDefault().Email
                    )
                );
            }

            var sortedLeague = ReturnSortedLeague(standings);
            var sortedLeagueWithPositions = AddPositionInLeagueToList(sortedLeague);

            return sortedLeagueWithPositions;
        }

        public SingleLeagueMatchModel GetSingleLeagueMatchById(int matchId)
        {
            return _context.SingleLeagueMatches.FirstOrDefault(f => f.Id == matchId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateSingleLeagueMatch(SingleLeagueMatchModel match)
        {
            // Do nothing
        }

        private List<int> GetAllUsersOfLeague(int leagueId)
        {
            List<int> userIds = new();
            var allPlayersInLeague = _context.LeaguePlayers
                .Where(x => x.LeagueId == leagueId)
                .Select(s => new SingleLeagueStandingsAllPlayersQuery
                {
                    Id = s.Id,
                    UserId = s.UserId
                }).ToList();

            foreach (SingleLeagueStandingsAllPlayersQuery element in allPlayersInLeague)
            {
                userIds.Add(element.UserId);
            }
            return userIds;
        }

        private List<SingleLeagueStandingsQuery> ReturnSortedLeague(List<SingleLeagueStandingsQuery> singleLeagueStandings)
        {
            return singleLeagueStandings.OrderByDescending(x => x.Points).ToList();
        }

        private List<SingleLeagueStandingsQuery> AddPositionInLeagueToList(List<SingleLeagueStandingsQuery> standings)
        {
            List<SingleLeagueStandingsQuery> result = standings;
            foreach (var item in result.Select((value, i) => new { i, value }))
            {
                item.value.PositionInLeague = item.i + 1;
            }
            return result;
        }

        public async Task<IEnumerable<SingleLeagueMathModelDapper>> TestDapper(CancellationToken ct)
        {
            var tx = await _context.Database.BeginTransactionAsync();

            var dapperReadData = await _context.QueryAsync<SingleLeagueMathModelDapper>(ct, @"
                SELECT slm.id, slm.player_one, slm.player_two, slm.match_started, slm.match_ended, slm.league_id 
                FROM single_league_matches slm");

            return dapperReadData;
        }

        public async Task<SingleLeagueMatchModel> ResetMatch(SingleLeagueMatchModel singleLeagueMatchModel, int matchId)
        {
            CancellationToken ct = new();
            if (singleLeagueMatchModel == null)
                throw new ArgumentNullException(nameof(singleLeagueMatchModel));

            var allGoals = _context.SingleLeagueGoals.Where(x => x.MatchId == matchId).ToList();

            var tx = await _context.Database.BeginTransactionAsync(ct);

            string sqlString = $"DELETE FROM single_league_goals WHERE match_id = {matchId}";

            await _context.ExecuteAsync(ct, sqlString);

            await tx.CommitAsync(ct);

            singleLeagueMatchModel.StartTime = null;
            singleLeagueMatchModel.EndTime = null;
            singleLeagueMatchModel.PlayerOneScore = 0;
            singleLeagueMatchModel.PlayerTwoScore = 0;
            singleLeagueMatchModel.MatchStarted = false;
            singleLeagueMatchModel.MatchEnded = false;
            singleLeagueMatchModel.MatchPaused = false;

            _context.SingleLeagueMatches.Update(singleLeagueMatchModel);
            _context.SaveChanges();

            return singleLeagueMatchModel;
        }
    }
}