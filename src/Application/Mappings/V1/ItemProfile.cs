using AutoMapper;
using Template.Application.Models.DTOs.V1.Items;
using Template.Domain.Entities.V1;

namespace Template.Application.Mappings.V1
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, ItemDetailDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Item, ItemExportDto>()
               .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Item, ItemListDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<ItemCreateDto, Item>();
            CreateMap<ItemUpdateDto, Item>();
        }
    }
}