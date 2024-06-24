using AutoMapper;
using Template.Application.Models.DTOs.V1.Category;
using Template.Domain.Entities.V1;

namespace Template.Application.Mappings.V1
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {            CreateMap<Category, CategoryListDTO>();
            CreateMap<Category, CategoryExportDTO>();
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<Category, CategoryDetailDTO>();
        }
    }
}