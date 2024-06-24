using AutoMapper;
using Template.Application.Models.DTOs.V1.Item;
using Template.Domain.Entities.V1;

namespace Template.Application.Mappings.V1
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {            CreateMap<Item, ItemDetailDTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Item, ItemExportDTO>()
               .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Item, ItemListDTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<ItemCreateDTO, Item>();
            CreateMap<ItemUpdateDTO, Item>();
        }
    }
}