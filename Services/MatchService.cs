using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Models.Matches;
using System.Linq;

namespace FoosballApi.Services
{
    public interface IMatchService
    {
        IEnumerable<FreehandMatchModel> GetAllFreehandMatches(int userId);
    }
    public class MatchService : IMatchService
    {
        private readonly DataContext _context;

        public MatchService(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<FreehandMatchModel> GetAllFreehandMatches(int userId)
        {
            var query = from fm in _context.FreehandMatches
                        where fm.PlayerOneId == userId || fm.PlayerTwoId == userId
                        select fm;

            return query.ToList();
        }
    }
}