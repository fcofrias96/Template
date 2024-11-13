using AutoMapper;
using Template.Application.Models.DTOs.V1.Actions;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Mappings.V1
{
    public class ActionProfile : Profile
    {
        public ActionProfile()
        {
            CreateMap<ActionType, ActionListDto>();
        }
    }
}