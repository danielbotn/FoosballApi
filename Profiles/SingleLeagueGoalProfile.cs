using AutoMapper;
using FoosballApi.Dtos.SingleLeagueGoals;
using FoosballApi.Models.SingleLeagueGoals;

namespace FoosballApi.Profiles
{
    public class SingleLeagueGoalProfile : Profile
    {
        public SingleLeagueGoalProfile()
        {
            CreateMap<SingleLeagueGoalModel, SingleLeagueGoalReadDto>();
            CreateMap<SingleLeagueGoalReadDto, SingleLeagueGoalModel>();
        }
    }
}