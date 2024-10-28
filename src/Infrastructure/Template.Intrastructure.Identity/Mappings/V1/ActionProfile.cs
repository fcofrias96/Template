using AutoMapper;
using Template.Application.Models.DTOs.V1.Actions;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Mappings.V1
{
    public class ActionProfile : Profile
    {
        public ActionProfile()
        {
            CreateMap<Action, ActionListDto>();
        }
    }
}
