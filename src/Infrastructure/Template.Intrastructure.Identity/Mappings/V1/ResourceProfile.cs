using AutoMapper;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Identity.Entities;

namespace Template.Identity.Mappings.V1
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {            CreateMap<Resource, ResourceListDto>();
            CreateMap<ResourceCreateDto, Resource>();
            CreateMap<ResourceUpdateDTO, Resource>();
            CreateMap<Resource, ResourceDetailDto>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions
                                                                .Select(p => $"{p.Action.Name} {p.Resource.Name}")));
        }
    }
}