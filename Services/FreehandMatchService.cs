using System.Collections.Generic;
using FoosballApi.Data;
using FoosballApi.Models.Matches;
using System.Linq;
using FoosballApi.Dtos.Matches;
using System;
using FoosballApi.Models;

namespace FoosballApi.Services
{
    public interface IFreehandMatchService
    {
        IEnumerable<FreehandMatchModelExtended> GetAllFreehandMatches(int userId);
        FreehandMatchModel CreateFreehandMatch(int userId, int organisationId, FreehandMatchCreateDto freehandMatchCreateDto);
        FreehandMatchModelExtended GetFreehandMatchById(int matchId);
        FreehandMatchModel GetFreehandMatchByIdFromDatabase(int matchId);
        bool CheckFreehandMatchPermission(int matchId, int userId);
        void UpdateFreehandMatch(FreehandMatchModel freehandMatchModel);
        bool SaveChanges();
        void DeleteFreehandMatch(FreehandMatchModel freehandMatchModel);
    }

    public class FreehandMatchService : IFreehandMatchService
    {
        private readonly DataContext _context;

        public FreehandMatchService(DataContext context)
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
                        break;
                    }
                }

                foreach (var p2Item in playerTwo)
                {
                    if (element.OrganisationId == p2Item.OrganisationId)
                    {
                        sameOrganisationAsPlayerTwo = true;
                        break;
                    }
                }
            }

            // User has permissions if both players belong to same organisation
            if (sameOrganisationAsPlayerOne && sameOrganisationAsPlayerTwo)
                return true;

            return false;
        }

        public FreehandMatchModel CreateFreehandMatch(int userId, int organisationId, FreehandMatchCreateDto freehandMatchCreateDto)
        {
            FreehandMatchModel fmm = new FreehandMatchModel();
            DateTime now = DateTime.Now;
            fmm.PlayerOneId = freehandMatchCreateDto.PlayerOneId;
            fmm.PlayerTwoId = freehandMatchCreateDto.PlayerTwoId;
            fmm.PlayerOneScore = freehandMatchCreateDto.PlayerOneScore;
            fmm.PlayerTwoScore = freehandMatchCreateDto.PlayerTwoScore;
            fmm.StartTime = now;
            fmm.GameFinished = freehandMatchCreateDto.GameFinished;
            fmm.GamePaused = freehandMatchCreateDto.GamePaused;
            fmm.UpTo = freehandMatchCreateDto.UpTo;
            fmm.OrganisationId = organisationId;
            _context.FreehandMatches.Add(fmm);
            _context.SaveChanges();

            return fmm;
        }

        public void DeleteFreehandMatch(FreehandMatchModel freehandMatchModel)
        {
            if (freehandMatchModel == null)
            {
                throw new ArgumentNullException(nameof(freehandMatchModel));
            }
            _context.FreehandMatches.Remove(freehandMatchModel);
        }

        public IEnumerable<FreehandMatchModelExtended> GetAllFreehandMatches(int userId)
        {
            var query = from fm in _context.FreehandMatches
                        where fm.PlayerOneId == userId || fm.PlayerTwoId == userId
                        select fm;
            var data = query.ToList();

            List<FreehandMatchModelExtended> freehandMatchModelExtendedList = new List<FreehandMatchModelExtended>();

            foreach (var item in data)
            {
                FreehandMatchModelExtended fmme = new FreehandMatchModelExtended{
                Id = item.Id,
                PlayerOneId = item.PlayerOneId,
                PlayerOneFirstName = _context.Users.FirstOrDefault(u => u.Id == item.PlayerOneId).FirstName,
                PlayerOneLastName = _context.Users.FirstOrDefault(u => u.Id == item.PlayerOneId).LastName,
                PlayerOnePhotoUrl = _context.Users.FirstOrDefault(u => u.Id == item.PlayerOneId).PhotoUrl,
                PlayerTwoId = item.PlayerTwoId,
                PlayerTwoFirstName = _context.Users.FirstOrDefault(u => u.Id == item.PlayerTwoId).FirstName,
                PlayerTwoLastName = _context.Users.FirstOrDefault(u => u.Id == item.PlayerTwoId).LastName,
                PlayerTwoPhotoUrl = _context.Users.FirstOrDefault(u => u.Id == item.PlayerTwoId).PhotoUrl,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                PlayerOneScore = item.PlayerOneScore,
                PlayerTwoScore = item.PlayerTwoScore,
                UpTo = item.UpTo,
                GameFinished = item.GameFinished,
                GamePaused = item.GamePaused,
                };
                freehandMatchModelExtendedList.Add(fmme);
            }

            return freehandMatchModelExtendedList;
        }

        public FreehandMatchModelExtended GetFreehandMatchById(int matchId)
        {
            var data = _context.FreehandMatches.FirstOrDefault(f => f.Id == matchId);
            FreehandMatchModelExtended fmme = new FreehandMatchModelExtended{
                Id = data.Id,
                PlayerOneId = data.PlayerOneId,
                PlayerOneFirstName = _context.Users.FirstOrDefault(u => u.Id == data.PlayerOneId).FirstName,
                PlayerOneLastName = _context.Users.FirstOrDefault(u => u.Id == data.PlayerOneId).LastName,
                PlayerOnePhotoUrl = _context.Users.FirstOrDefault(u => u.Id == data.PlayerOneId).PhotoUrl,
                PlayerTwoId = data.PlayerTwoId,
                PlayerTwoFirstName = _context.Users.FirstOrDefault(u => u.Id == data.PlayerTwoId).FirstName,
                PlayerTwoLastName = _context.Users.FirstOrDefault(u => u.Id == data.PlayerTwoId).LastName,
                PlayerTwoPhotoUrl = _context.Users.FirstOrDefault(u => u.Id == data.PlayerTwoId).PhotoUrl,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                PlayerOneScore = data.PlayerOneScore,
                PlayerTwoScore = data.PlayerTwoScore,
                UpTo = data.UpTo,
                GameFinished = data.GameFinished,
                GamePaused = data.GamePaused,
            };
            return fmme;
        }

        public FreehandMatchModel GetFreehandMatchByIdFromDatabase(int matchId)
        {
            return _context.FreehandMatches.FirstOrDefault(f => f.Id == matchId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateFreehandMatch(FreehandMatchModel freehandMatchModel)
        {
            // Do nothing
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
