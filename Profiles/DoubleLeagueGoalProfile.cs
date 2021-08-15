using AutoMapper;
using FoosballApi.Dtos.DoubleLeagueGoals;
using FoosballApi.Models.DoubleLeagueGoals;

namespace FoosballApi.Profiles
{
    public class DoubleLeagueGoalProfile : Profile
    {
        public DoubleLeagueGoalProfile()
        {
            CreateMap<DoubleLeagueGoalsReadDto, DoubleLeagueGoalsDapper>();
            CreateMap<DoubleLeagueGoalsDapper, DoubleLeagueGoalsReadDto>();
        }
    }
}