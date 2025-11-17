using Template.Application.Extensions;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Seeds
{
    public static class ActionSeed
    {
        public static List<ActionType> Data { get; } = Enum.GetValues(typeof(Application.Enums.ActionType))
        .Cast<Application.Enums.ActionType>()
        .Select(action => new ActionType
        {
            Id = action.ToInt(),
            Name = action.ToName(),
        })
        .ToList();
    }
}