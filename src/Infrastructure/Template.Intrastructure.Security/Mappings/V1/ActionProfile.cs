using AutoMapper;
using Template.Application.Models.DTOs;
using PermissionAction = Template.Domain.Entities.V1.PermissionAction;

namespace Template.Security.Mappings.V1
{
    public class ActionProfile : Profile
    {
        public ActionProfile()
        {
            CreateMap<PermissionAction, Catalog>();
        }
    }
}