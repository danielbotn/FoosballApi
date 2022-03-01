using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FoosballApi.Data;
using FoosballApi.Dtos.DoubleMatches;
using FoosballApi.Models;
using FoosballApi.Models.Matches;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FoosballApi.Services
{
    public interface IFreehandDoubleMatchService
    {
        IEnumerable<FreehandDoubleMatchModel> GetAllFreehandDoubleMatches(int userId);
        FreehandDoubleMatchModel GetFreehandDoubleMatchById(int matchId);
        FreehandDoubleMatchModelExtended GetFreehandDoubleMatchByIdExtended(int matchId);
        bool CheckMatchPermission(int userId, int matchId);
        FreehandDoubleMatchModel CreateFreehandDoubleMatch(int userId, FreehandDoubleMatchCreateDto freehandDoubleMatchCreateDto);
        void UpdateFreehandMatch(FreehandDoubleMatchModel freehandMatchModel);
        bool SaveChanges();
        void DeleteFreehandMatch(FreehandDoubleMatchModel freehandDoubleMatchModel);
    }

    public class FreehandDoubleMatchService : IFreehandDoubleMatchService
    {
        private readonly DataContext _context;

        public FreehandDoubleMatchService(DataContext context)
        {
            _context = context;
        }

        public bool CheckMatchPermission(int userId, int matchId)
        {
            var doubleMatchData = _context.FreehandDoubleMatches
                                    .Where(x => x.Id.Equals(matchId))
                                    .Select(dmd => new FreehandDoubleMatchModel
                                    {
                                        Id = dmd.Id,
                                        OrganisationId = dmd.OrganisationId
                                    }).FirstOrDefault();

            var currentUser = _context.Users
                                .Where(x => x.Id.Equals(userId))
                                .Select(u => new User
                                {
                                    Id = u.Id,
                                    CurrentOrganisationId = u.CurrentOrganisationId
                                }).FirstOrDefault();

            if (doubleMatchData.OrganisationId == currentUser.CurrentOrganisationId)
                return true;

            return false;
        }

        public FreehandDoubleMatchModel CreateFreehandDoubleMatch(int userId, FreehandDoubleMatchCreateDto freehandDoubleMatchCreateDto)
        {
            FreehandDoubleMatchModel fdm = new FreehandDoubleMatchModel();
            DateTime now = DateTime.Now;
            fdm.OrganisationId = freehandDoubleMatchCreateDto.OrganisationId;
            fdm.PlayerOneTeamA = freehandDoubleMatchCreateDto.PlayerOneTeamA;
            fdm.PlayerOneTeamB = freehandDoubleMatchCreateDto.PlayerOneTeamB;
            fdm.PlayerTwoTeamA = freehandDoubleMatchCreateDto.PlayerTwoTeamA;
            fdm.PlayerTwoTeamB = freehandDoubleMatchCreateDto.PlayerTwoTeamB;
            fdm.StartTime = now;

            if (!string.IsNullOrEmpty(freehandDoubleMatchCreateDto.NicknameTeamA))
                fdm.NicknameTeamA = freehandDoubleMatchCreateDto.NicknameTeamA;

            if (!string.IsNullOrEmpty(freehandDoubleMatchCreateDto.NicknameTeamB))
                fdm.NicknameTeamB = freehandDoubleMatchCreateDto.NicknameTeamB;

            if (freehandDoubleMatchCreateDto.UpTo != null)
                fdm.UpTo = freehandDoubleMatchCreateDto.UpTo;

            fdm.GameFinished = false;
            fdm.GamePaused = false;

            _context.FreehandDoubleMatches.Add(fdm);
            _context.SaveChanges();

            return fdm;
        }

        public void DeleteFreehandMatch(FreehandDoubleMatchModel freehandDoubleMatchModel)
        {
            if (freehandDoubleMatchModel == null)
            {
                throw new ArgumentNullException(nameof(freehandDoubleMatchModel));
            }
            _context.FreehandDoubleMatches.Remove(freehandDoubleMatchModel);
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

        public FreehandDoubleMatchModel GetFreehandDoubleMatchById(int matchId)
        {
            var match = _context.FreehandDoubleMatches.FirstOrDefault(x => x.Id == matchId);
            return match;
        }

        public FreehandDoubleMatchModelExtended GetFreehandDoubleMatchByIdExtended(int matchId)
        {
            var data = _context.FreehandDoubleMatches.FirstOrDefault(x => x.Id == matchId);
            TimeSpan? playingTime = null;
            if (data.EndTime != null) {
                playingTime = data.EndTime - data.StartTime;
            }
            FreehandDoubleMatchModelExtended fdme = new FreehandDoubleMatchModelExtended {
                Id = data.Id,
                PlayerOneTeamA = data.PlayerOneTeamA,
                PlayerOneTeamAFirstName = _context.Users.FirstOrDefault(x => x.Id == data.PlayerOneTeamA).FirstName,
                PlayerOneTeamALastName = _context.Users.FirstOrDefault(x => x.Id == data.PlayerOneTeamA).LastName,
                PlayerOneTeamAPhotoUrl = _context.Users.FirstOrDefault(x => x.Id == data.PlayerOneTeamA).PhotoUrl,
                PlayerTwoTeamA = data.PlayerTwoTeamA,
                PlayerTwoTeamAFirstName = _context.Users.FirstOrDefault(x => x.Id == data.PlayerTwoTeamA).FirstName,
                PlayerTwoTeamALastName = _context.Users.FirstOrDefault(x => x.Id == data.PlayerTwoTeamA).LastName,
                PlayerTwoTeamAPhotoUrl = _context.Users.FirstOrDefault(x => x.Id == data.PlayerTwoTeamA).PhotoUrl,
                PlayerOneTeamB = data.PlayerOneTeamB,
                PlayerOneTeamBFirstName = _context.Users.FirstOrDefault(x => x.Id == data.PlayerOneTeamB).FirstName,
                PlayerOneTeamBLastName = _context.Users.FirstOrDefault(x => x.Id == data.PlayerOneTeamB).LastName,
                PlayerOneTeamBPhotoUrl = _context.Users.FirstOrDefault(x => x.Id == data.PlayerOneTeamB).PhotoUrl,
                PlayerTwoTeamB = data.PlayerTwoTeamB,
                PlayerTwoTeamBFirstName = _context.Users.FirstOrDefault(x => x.Id == data.PlayerTwoTeamB).FirstName,
                PlayerTwoTeamBLastName = _context.Users.FirstOrDefault(x => x.Id == data.PlayerTwoTeamB).LastName,
                PlayerTwoTeamBPhotoUrl = _context.Users.FirstOrDefault(x => x.Id == data.PlayerTwoTeamB).PhotoUrl,
                OrganisationId = data.OrganisationId,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                TotalPlayingTime = playingTime != null ? ToReadableAgeString(playingTime.Value) : null,
                TeamAScore = data.TeamAScore,
                TeamBScore = data.TeamBScore,
                NicknameTeamA = data.NicknameTeamA,
                NicknameTeamB = data.NicknameTeamB,
                UpTo = data.UpTo,
                GameFinished = data.GameFinished,
                GamePaused = data.GamePaused
            };
            return fdme;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateFreehandMatch(FreehandDoubleMatchModel freehandMatchModel)
        {
            // Do nothing
        }

        public string ToReadableAgeString(TimeSpan span)
        {
            return string.Format("{0:hh\\:mm\\:ss}", span);
        }

    }
}