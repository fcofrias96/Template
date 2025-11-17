using AutoMapper;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Categories;
using Template.Domain.Entities.V1;

namespace Template.Application.Mappings.V1
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryListDto>();
            CreateMap<Category, CategoryExportDto>();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
            CreateMap<Category, CategoryDetailDto>();
            CreateMap<Category, Catalog>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}