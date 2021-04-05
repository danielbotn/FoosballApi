using AutoMapper;
using FoosballApi.Dtos.Organisations;
using FoosballApi.Dtos.Users;
using FoosballApi.Models;

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
        }

    }
}