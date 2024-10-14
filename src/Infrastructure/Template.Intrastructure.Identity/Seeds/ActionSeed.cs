
using Action = Template.Identity.Entities.Action;
using Template.Application.Extensions;
using Template.Application.Enums;

namespace Template.Identity.Seeds
{
    public static class ActionSeed
    {
        public static List<Action> Data { get; } = Enum.GetValues(typeof(Actions))
        .Cast<Actions>()
        .Select(action => new Action
        {
            Id = action.ToInt(),
            Name = action.ToName(),
        })
        .ToList();
    }
}