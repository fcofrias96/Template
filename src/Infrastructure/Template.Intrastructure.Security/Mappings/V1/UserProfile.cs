using AutoMapper;
using Template.Application.Models.DTOs.V1.Users;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Mappings.V1
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateDto, ApplicationUser>();
            CreateMap<UserUpdateDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserListDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)));
        }
    }
}