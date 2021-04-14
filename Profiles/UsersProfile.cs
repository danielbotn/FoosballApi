using AutoMapper;
using FoosballApi.Dtos.Leagues;
using FoosballApi.Dtos.Organisations;
using FoosballApi.Dtos.Users;
using FoosballApi.Models;
using FoosballApi.Models.Leagues;
using FoosballApi.Models.Organisations;

namespace FoosballApi.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, UserReadDto>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<User, UserUpdateDto>();
            CreateMap<UserCreateDto, User>();

            CreateMap<OrganisationModel, OrganisationReadDto>();
            CreateMap<OrganisationUpdateDto, OrganisationModel>();

            CreateMap<LeagueModel, LeagueReadDto>();
            CreateMap<LeagueReadDto, LeagueModel>();

            CreateMap<LeaguePlayersModel, LeaguePlayersReadDto>();
            CreateMap<LeaguePlayersReadDto, LeaguePlayersModel>();

            CreateMap<LeaguePlayersJoinModel, LeaguePlayersReadDto>();
            CreateMap<LeaguePlayersReadDto, LeaguePlayersJoinModel>();

            CreateMap<OrganisationReadDto, OrganisationModelCreate>();
            CreateMap<OrganisationModelCreate, OrganisationReadDto>();

            CreateMap<LeagueUpdateDto, LeagueModel>();
            CreateMap<LeagueModel, LeagueUpdateDto>();
        }

    }
}