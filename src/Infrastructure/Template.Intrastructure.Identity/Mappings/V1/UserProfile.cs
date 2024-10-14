using AutoMapper;
using Template.Application.Models.DTOs.V1.User;
using Template.Identity.Entities;

namespace Template.Identity.Mappings.V1
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateDTO, User>();
            CreateMap<UserUpdateDTO, User>();
            CreateMap<User, UserDetailDTO>();
            CreateMap<User, UserListDTO>();
        }
    }
}