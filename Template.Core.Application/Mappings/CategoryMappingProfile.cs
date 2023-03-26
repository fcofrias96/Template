using AutoMapper;
using Template.Core.Application.DTOs.Category;
using Template.Core.Domain.Entities;

namespace Template.Core.Application.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {            CreateMap<CategoryListDTO, Category>().ReverseMap();
            CreateMap<CategoryCreateDTO, Category>().ReverseMap();
            CreateMap<CategoryUpdateDTO, Category>().ReverseMap();
            CreateMap<CategoryDetailsDTO, Category>().ReverseMap();
        }
    }
}