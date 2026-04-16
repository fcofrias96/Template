using System.Collections.Generic;
using PermissionAction = Template.Security.Entities.PermissionAction;

namespace Template.Security.Seeds
{
    public static class ActionSeed
    {
        public static List<PermissionAction> Data { get; } = new List<PermissionAction>
        {
            new PermissionAction { Id = PermissionAction.Create, Name = nameof(PermissionAction.Create) },
            new PermissionAction { Id = PermissionAction.Read, Name = nameof(PermissionAction.Read) },
            new PermissionAction { Id = PermissionAction.Update, Name = nameof(PermissionAction.Update) },
            new PermissionAction { Id = PermissionAction.Delete, Name = nameof(PermissionAction.Delete) },
            new PermissionAction { Id = PermissionAction.Export, Name = nameof(PermissionAction.Export) },
        };
    }
}