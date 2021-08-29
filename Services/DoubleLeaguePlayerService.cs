using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FoosballApi.Data;
using FoosballApi.Models.DoubleLeaguePlayers;

namespace FoosballApi.Services
{
    public interface IDoubleLeaguePlayerService
    {
        Task<IEnumerable<DoubleLeaguePlayerModelDapper>> GetDoubleLeaguePlayersyLeagueId(int leagueId);
        Task<DoubleLeaguePlayerModelDapper> GetDoubleLeaguePlayerById(int playerId);
    }

    public class DoubleLeaguePlayerService : IDoubleLeaguePlayerService
    {
        private readonly DataContext _context;

        public DoubleLeaguePlayerService(DataContext context)
        {
            _context = context;
        }

        public async Task<DoubleLeaguePlayerModelDapper> GetDoubleLeaguePlayerById(int playerId)
        {
            CancellationToken ct = new();

            var tx = await _context.Database.BeginTransactionAsync();

            var dapperReadData = await _context.QueryAsync<DoubleLeaguePlayerModelDapper>(ct, @"
                select dlp.id, dlp.user_id, dlp.double_league_team_id DoubleLeagueTeamId, u.first_name FirstName, 
                u.last_name LastName, u.email, dlt.id as teamId, dlt.name as team_name
                from double_league_players dlp
                join double_league_teams dlt on dlp.double_league_team_id = dlt.id
                join users u on u.id = dlp.user_id " + $"where dlp.id = {playerId}");

            return dapperReadData.FirstOrDefault();
        }

        public async Task<IEnumerable<DoubleLeaguePlayerModelDapper>> GetDoubleLeaguePlayersyLeagueId(int leagueId)
        {
            CancellationToken ct = new();

            var tx = await _context.Database.BeginTransactionAsync();

            var dapperReadData = await _context.QueryAsync<DoubleLeaguePlayerModelDapper>(ct, @"
                select dlp.id, dlp.user_id, dlp.double_league_team_id DoubleLeagueTeamId, u.first_name FirstName, 
                u.last_name LastName, u.email, dlt.id as teamId, dlt.name as team_name from double_league_players dlp
                join double_league_teams dlt on dlp.double_league_team_id = dlt.id
                join users u on u.id = dlp.user_id " + $"where dlt.league_id = {leagueId}");

            return dapperReadData;
        }
    }
}