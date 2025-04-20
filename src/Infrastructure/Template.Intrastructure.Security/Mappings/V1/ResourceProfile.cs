using AutoMapper;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Security.Entities;
using Template.Application.Models.DTOs;

namespace Template.Security.Mappings.V1
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {            CreateMap<Resource, Dropdown>()
                .ForMember(dest=> dest.Id, opt=> opt.MapFrom(src=> src.Id.ToString()));
            CreateMap<Resource, ResourceListDto>();
            CreateMap<ResourceCreateDto, Resource>();
            CreateMap<ResourceUpdateDto, Resource>();
            CreateMap<Resource, ResourceDetailDto>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions
                                                                .Select(p => $"{p.ActionType.Name} {p.Resource.Name}")));
        }
    }
}