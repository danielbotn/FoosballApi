using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Dtos.SingleLeagueMatches;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FoosballApi.Models.Matches;

namespace FoosballApi.Services
{
    public interface ISingleLeagueMatchService
    {
        IEnumerable<SingleLeagueMatchModel> GetAllMatchesByOrganisationId(int organisationId, int leagueId);
    }
    public class SingleLeagueMatchService : ISingleLeagueMatchService
    {
        private readonly DataContext _context;

        public SingleLeagueMatchService(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<SingleLeagueMatchModel> GetAllMatchesByOrganisationId(int organisationId, int leagueId)
        {
            // var query = _context.SingleLeagueMatches.FromSqlRaw("SELECT * FROM dbo.SingleLeagueMatches");
            var query = _context.SingleLeagueMatches.FromSqlRaw("SELECT * FROM \"single_league_matches\"").ToList();


            var data = query.ToList();

            return null;

        }
    }
}