using AutoMapper;
using Template.Application.Models.DTOs.V1.Users;
using Template.Identity.Entities;

namespace Template.Identity.Mappings.V1
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<User, UserListDto>()
                .ForMember(dest=> dest.Roles, opt=> opt.MapFrom(src=> src.UserRoles.Select(ur=> ur.Role.Name)));
           
        }
    }
}
