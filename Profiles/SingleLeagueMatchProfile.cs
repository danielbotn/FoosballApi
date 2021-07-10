using AutoMapper;
using FoosballApi.Dtos.SingleLeagueMatches;
using FoosballApi.Models.Matches;

namespace FoosballApi.Profiles
{
    public class SingleLeagueMatchProfile : Profile
    {
        public SingleLeagueMatchProfile()
        {
            CreateMap<SingleLeagueMatchModel, SingleLeagueMatchReadDto>();
            CreateMap<SingleLeagueMatchReadDto, SingleLeagueMatchModel>();
            CreateMap<SingleLeagueMatchModel, SingleLeagueMatchUpdateDto>();
            CreateMap<SingleLeagueMatchUpdateDto, SingleLeagueMatchModel>();
        }
    }
}