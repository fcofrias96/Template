using AutoMapper;
using Template.Core.Application.DTOs.User;
using Template.Infrastructure.Identity.Entity;

namespace Template.Infrastructure.Identity.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<UserCreateDTO, User>().ReverseMap();
            CreateMap<UserUpdateDTO, User>().ReverseMap();
            CreateMap<UserDetailDTO, User>().ReverseMap();
            CreateMap<UserListDTO, User>().ReverseMap();
        }
    }
}