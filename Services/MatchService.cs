using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Models.Matches;
using System.Linq;
using FoosballApi.Dtos.Matches;
using System;
using FoosballApi.Models;

namespace FoosballApi.Services
{
    public interface IMatchService
    {
        IEnumerable<FreehandMatchModel> GetAllFreehandMatches(int userId);

        void CreateFreehandMatch(int userId, FreehandMatchCreateDto freehandMatchCreateDto);

        FreehandMatchModel GetFreehandMatchById(int matchId);

        bool CheckFreehandMatchPermission(int matchId, int userId);
    }
    public class MatchService : IMatchService
    {
        private readonly DataContext _context;

        public MatchService(DataContext context)
        {
            _context = context;
        }

        // Returns true if current user belongs to same organisation as player one and player two
        public bool CheckFreehandMatchPermission(int matchId, int userId)
        {
            var query = from fm in _context.FreehandMatches
                        where fm.Id == matchId
                        select new FreehandPermissionModel
                        {
                            MatchId = fm.Id,
                            PlayerOneId = fm.PlayerOneId,
                            PlayerTwoId = fm.PlayerTwoId
                        };

            var queryData = query.FirstOrDefault();

            if (queryData == null)
                return false;

            IEnumerable<OrganisationListModel> currentUser = GetAllOrganisationsOfUser(userId);

            IEnumerable<OrganisationListModel> playerOne = GetAllOrganisationsOfUser(queryData.PlayerOneId);

            IEnumerable<OrganisationListModel> playerTwo = GetAllOrganisationsOfUser(queryData.PlayerTwoId);

            bool sameOrganisationAsPlayerOne = false;
            bool sameOrganisationAsPlayerTwo = false;

            foreach (var element in currentUser)
            {
                foreach (var p1Item in playerOne)
                {
                    if (element.OrganisationId == p1Item.OrganisationId)
                    {
                        sameOrganisationAsPlayerOne = true;
                    }
                }

                foreach (var p2Item in playerTwo)
                {
                    if (element.OrganisationId == p2Item.OrganisationId)
                    {
                        sameOrganisationAsPlayerTwo = true;
                    }
                }
            }

            // User has permissions if both players belong to same organisation
            if (sameOrganisationAsPlayerOne && sameOrganisationAsPlayerTwo)
                return true;

            return false;
        }

        public void CreateFreehandMatch(int userId, FreehandMatchCreateDto freehandMatchCreateDto)
        {
            FreehandMatchModel fmm = new FreehandMatchModel();
            DateTime now = DateTime.Now;
            fmm.PlayerOneId = freehandMatchCreateDto.PlayerOneId;
            fmm.PlayerTwoId = freehandMatchCreateDto.PlayerTwoId;
            fmm.PlayerOneScore = freehandMatchCreateDto.PlayerOneScore;
            fmm.PlayerTwoScore = freehandMatchCreateDto.PlayerTwoScore;
            fmm.StartTime = now;
            fmm.GameFinished = freehandMatchCreateDto.GameFinished;
            fmm.GamePaused = freehandMatchCreateDto.GameFinished;
            fmm.UpTo = freehandMatchCreateDto.UpTo;
            _context.FreehandMatches.Add(fmm);
            _context.SaveChanges();
        }

        public IEnumerable<FreehandMatchModel> GetAllFreehandMatches(int userId)
        {
            var query = from fm in _context.FreehandMatches
                        where fm.PlayerOneId == userId || fm.PlayerTwoId == userId
                        select fm;

            return query.ToList();
        }

        public FreehandMatchModel GetFreehandMatchById(int matchId)
        {
            return _context.FreehandMatches.FirstOrDefault(f => f.Id == matchId);
        }

        private IEnumerable<OrganisationListModel> GetAllOrganisationsOfUser(int userId)
        {
            var query = from org in _context.OrganisationList
                        where org.UserId == userId
                        select org;

            return query.ToList();
        }

    }
}

// elisabeth
// kolbrun
// solvi 
// lena
// Desmond
// Fridbjorn