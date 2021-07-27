using AutoMapper;
using FoosballApi.Dtos.DoubleLeagueMatches;
using FoosballApi.Models.DoubleLeagueMatches;

namespace FoosballApi.Profiles
{
    public class DoubleLeagueMatchProfile : Profile
    {
        public DoubleLeagueMatchProfile()
        {
            CreateMap<AllMatchesModelReadDto, AllMatchesModel>();
            CreateMap<AllMatchesModel, AllMatchesModelReadDto>();
        }
    }
}