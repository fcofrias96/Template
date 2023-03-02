using AutoMapper;
using Template.Core.Application.DTOs.Item;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Mappings
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Item, ItemResponseDTO>().ReverseMap();

        }
    }
}