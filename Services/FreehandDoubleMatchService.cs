using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FoosballApi.Data;
using FoosballApi.Dtos.DoubleMatches;
using FoosballApi.Models.Matches;
using Microsoft.AspNetCore.Mvc;

namespace FoosballApi.Services
{
    public interface IFreehandDoubleMatchService
    {
        IEnumerable<FreehandDoubleMatchModel> GetAllFreehandDoubleMatches(int userId);

    }
    public class FreehandDoubleMatchService : IFreehandDoubleMatchService
    {
        private readonly DataContext _context;

        public FreehandDoubleMatchService(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<FreehandDoubleMatchModel> GetAllFreehandDoubleMatches(int userId)
        {
            var matches = _context.FreehandDoubleMatches
                .Where(b => b.UserPlayerOneTeamA.Id.Equals(userId) 
                    || b.UserPlayerOneTeamB.Id.Equals(userId) 
                    || b.UserPlayerTwoTeamA.Id.Equals(userId) 
                    || b.UserPlayerTwoTeamB.Id.Equals(userId))
                .ToList();

            return matches;
        }
    }
}