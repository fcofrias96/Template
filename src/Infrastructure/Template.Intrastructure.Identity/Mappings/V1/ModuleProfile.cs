using AutoMapper;
using Template.Application.Models.DTOs.V1.Module;
using Template.Identity.Entities;

namespace Template.Identity.Mappings.V1
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {            CreateMap<Module, ModuleListDTO>();
            CreateMap<ModuleCreateDTO, Module>();
            CreateMap<ModuleUpdateDTO, Module>();
            CreateMap<Module, ModuleDetailDTO>();
        }
    }
}