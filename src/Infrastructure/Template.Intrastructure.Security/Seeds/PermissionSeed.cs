using Template.Application.Enums;
using Template.Application.Extensions;
using Template.Security.Entities;
using ActionType = Template.Application.Enums.ActionType;

namespace Template.Security.Seeds
{
    public static class PermissionSeed
    {
        public static List<Permission> Data { get; } = new List<Permission>
        {
            #region User

            new Permission
            {
                Id            = 1,
                ActionTypeId  = ActionType.Create.ToInt(),
                ResourceId    = Resources.User.ToInt(),
                Name          = "Create User",
                Description   = "Allows creating users"
            },
            new Permission
            {
                Id            = 2,
                ActionTypeId  = ActionType.Read.ToInt(),
                ResourceId    = Resources.User.ToInt(),
                Name          = "Read User",
                Description   = "Allows reading user details"
            },
            new Permission
            {
                Id            = 3,
                ActionTypeId  = ActionType.Update.ToInt(),
                ResourceId    = Resources.User.ToInt(),
                Name          = "Update User",
                Description   = "Allows updating users"
            },
            new Permission
            {
                Id            = 4,
                ActionTypeId  = ActionType.Delete.ToInt(),
                ResourceId    = Resources.User.ToInt(),
                Name          = "Delete User",
                Description   = "Allows deleting users"
            },
            new Permission
            {
                Id            = 5,
                ActionTypeId  = ActionType.Export.ToInt(),
                ResourceId    = Resources.User.ToInt(),
                Name          = "Export User",
                Description   = "Allows exporting user data"
            },

            #endregion User

            #region Resource

            new Permission
            {
                Id            = 6,
                ActionTypeId  = ActionType.Create.ToInt(),
                ResourceId    = Resources.Resource.ToInt(),
                Name          = "Create Resource",
                Description   = "Allows creating resources"
            },
            new Permission
            {
                Id            = 7,
                ActionTypeId  = ActionType.Read.ToInt(),
                ResourceId    = Resources.Resource.ToInt(),
                Name          = "Read Resource",
                Description   = "Allows reading resources"
            },
            new Permission
            {
                Id            = 8,
                ActionTypeId  = ActionType.Update.ToInt(),
                ResourceId    = Resources.Resource.ToInt(),
                Name          = "Update Resource",
                Description   = "Allows updating resources"
            },
            new Permission
            {
                Id            = 9,
                ActionTypeId  = ActionType.Delete.ToInt(),
                ResourceId    = Resources.Resource.ToInt(),
                Name          = "Delete Resource",
                Description   = "Allows deleting resources"
            },
            new Permission
            {
                Id            = 10,
                ActionTypeId  = ActionType.Export.ToInt(),
                ResourceId    = Resources.Resource.ToInt(),
                Name          = "Export Resource",
                Description   = "Allows exporting resource data"
            },

            #endregion Resource

            #region Role

            new Permission
            {
                Id            = 11,
                ActionTypeId  = ActionType.Create.ToInt(),
                ResourceId    = Resources.Role.ToInt(),
                Name          = "Create Role",
                Description   = "Allows creating roles"
            },
            new Permission
            {
                Id            = 12,
                ActionTypeId  = ActionType.Read.ToInt(),
                ResourceId    = Resources.Role.ToInt(),
                Name          = "Read Role",
                Description   = "Allows reading roles"
            },
            new Permission
            {
                Id            = 13,
                ActionTypeId  = ActionType.Update.ToInt(),
                ResourceId    = Resources.Role.ToInt(),
                Name          = "Update Role",
                Description   = "Allows updating roles"
            },
            new Permission
            {
                Id            = 14,
                ActionTypeId  = ActionType.Delete.ToInt(),
                ResourceId    = Resources.Role.ToInt(),
                Name          = "Delete Role",
                Description   = "Allows deleting roles"
            },
            new Permission
            {
                Id            = 15,
                ActionTypeId  = ActionType.Export.ToInt(),
                ResourceId    = Resources.Role.ToInt(),
                Name          = "Export Role",
                Description   = "Allows exporting role data"
            },

            #endregion Role

            #region Permission

            new Permission
            {
                Id            = 16,
                ActionTypeId  = ActionType.Create.ToInt(),
                ResourceId    = Resources.Permission.ToInt(),
                Name          = "Create Permission",
                Description   = "Allows creating permissions"
            },
            new Permission
            {
                Id            = 17,
                ActionTypeId  = ActionType.Read.ToInt(),
                ResourceId    = Resources.Permission.ToInt(),
                Name          = "Read Permission",
                Description   = "Allows reading permissions"
            },
            new Permission
            {
                Id            = 18,
                ActionTypeId  = ActionType.Update.ToInt(),
                ResourceId    = Resources.Permission.ToInt(),
                Name          = "Update Permission",
                Description   = "Allows updating permissions"
            },
            new Permission
            {
                Id            = 19,
                ActionTypeId  = ActionType.Delete.ToInt(),
                ResourceId    = Resources.Permission.ToInt(),
                Name          = "Delete Permission",
                Description   = "Allows deleting permissions"
            },
            new Permission
            {
                Id            = 20,
                ActionTypeId  = ActionType.Export.ToInt(),
                ResourceId    = Resources.Permission.ToInt(),
                Name          = "Export Permission",
                Description   = "Allows exporting permission data"
            },

            #endregion Permission

            #region Action

            new Permission
            {
                Id            = 21,
                ActionTypeId  = ActionType.Read.ToInt(),
                ResourceId    = Resources.Action.ToInt(),
                Name          = "Read Action",
                Description   = "Allows reading actions"
            },

            #endregion Action

            #region Category

            new Permission
            {
                Id            = 22,
                ActionTypeId  = ActionType.Create.ToInt(),
                ResourceId    = Resources.Category.ToInt(),
                Name          = "Create Category",
                Description   = "Allows creating categories"
            },
            new Permission
            {
                Id            = 23,
                ActionTypeId  = ActionType.Read.ToInt(),
                ResourceId    = Resources.Category.ToInt(),
                Name          = "Read Category",
                Description   = "Allows reading categories"
            },
            new Permission
            {
                Id            = 24,
                ActionTypeId  = ActionType.Update.ToInt(),
                ResourceId    = Resources.Category.ToInt(),
                Name          = "Update Category",
                Description   = "Allows updating categories"
            },
            new Permission
            {
                Id            = 25,
                ActionTypeId  = ActionType.Delete.ToInt(),
                ResourceId    = Resources.Category.ToInt(),
                Name          = "Delete Category",
                Description   = "Allows deleting categories"
            },
            new Permission
            {
                Id            = 26,
                ActionTypeId  = ActionType.Export.ToInt(),
                ResourceId    = Resources.Category.ToInt(),
                Name          = "Export Category",
                Description   = "Allows exporting category data"
            },

            #endregion Category

            #region Item

            new Permission
            {
                Id            = 27,
                ActionTypeId  = ActionType.Create.ToInt(),
                ResourceId    = Resources.Item.ToInt(),
                Name          = "Create Item",
                Description   = "Allows creating items"
            },
            new Permission
            {
                Id            = 28,
                ActionTypeId  = ActionType.Read.ToInt(),
                ResourceId    = Resources.Item.ToInt(),
                Name          = "Read Item",
                Description   = "Allows reading items"
            },
            new Permission
            {
                Id            = 29,
                ActionTypeId  = ActionType.Update.ToInt(),
                ResourceId    = Resources.Item.ToInt(),
                Name          = "Update Item",
                Description   = "Allows updating items"
            },
            new Permission
            {
                Id            = 30,
                ActionTypeId  = ActionType.Delete.ToInt(),
                ResourceId    = Resources.Item.ToInt(),
                Name          = "Delete Item",
                Description   = "Allows deleting items"
            },
            new Permission
            {
                Id            = 31,
                ActionTypeId  = ActionType.Export.ToInt(),
                ResourceId    = Resources.Item.ToInt(),
                Name          = "Export Item",
                Description   = "Allows exporting item data"
            },

            #endregion Item

            #region Configuration

            new Permission
            {
                Id            = 32,
                ActionTypeId  = ActionType.Read.ToInt(),
                ResourceId    = Resources.Configuration.ToInt(),
                Name          = "Read Configuration",
                Description   = "Allows reading configuration settings"
            }

            #endregion Configuration
        };
    }
}