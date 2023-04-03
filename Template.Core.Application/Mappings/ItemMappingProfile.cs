using AutoMapper;
using Template.Core.Application.DTOs.Item;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Mappings
{
    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {            CreateMap<ItemListDTO, Item>().ReverseMap();
            CreateMap<ItemCreateDTO, Item>().ReverseMap();
            CreateMap<ItemUpdateDTO, Item>().ReverseMap();
            CreateMap<ItemDetailsDTO, Item>().ReverseMap();
        }
    }
}