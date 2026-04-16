using Template.Application.Enums;
using Template.Application.Extensions;
using Template.Domain.Entities.V1;

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
                ActionId  = PermissionAction.Create,
                ResourceId    = Resource.User,
                Name          = "Create User",
                Description   = "Allows creating users"
            },
            new Permission
            {
                Id            = 2,
                ActionId  = PermissionAction.Read,
                ResourceId    = Resource.User,
                Name          = "Read User",
                Description   = "Allows reading user details"
            },
            new Permission
            {
                Id            = 3,
                ActionId  = PermissionAction.Update,
                ResourceId    = Resource.User,
                Name          = "Update User",
                Description   = "Allows updating users"
            },
            new Permission
            {
                Id            = 4,
                ActionId  = PermissionAction.Delete,
                ResourceId    = Resource.User,
                Name          = "Delete User",
                Description   = "Allows deleting users"
            },
            new Permission
            {
                Id            = 5,
                ActionId  = PermissionAction.Export,
                ResourceId    = Resource.User,
                Name          = "Export User",
                Description   = "Allows exporting user data"
            },

            #endregion User

            #region Resource

            new Permission
            {
                Id            = 6,
                ActionId  = PermissionAction.Create,
                ResourceId    = Resource.ResourceItem,
                Name          = "Create Resource",
                Description   = "Allows creating resources"
            },
            new Permission
            {
                Id            = 7,
                ActionId  = PermissionAction.Read,
                ResourceId    = Resource.ResourceItem,
                Name          = "Read Resource",
                Description   = "Allows reading resources"
            },
            new Permission
            {
                Id            = 8,
                ActionId  = PermissionAction.Update,
                ResourceId    = Resource.ResourceItem,
                Name          = "Update Resource",
                Description   = "Allows updating resources"
            },
            new Permission
            {
                Id            = 9,
                ActionId  = PermissionAction.Delete,
                ResourceId    = Resource.ResourceItem,
                Name          = "Delete Resource",
                Description   = "Allows deleting resources"
            },
            new Permission
            {
                Id            = 10,
                ActionId  = PermissionAction.Export,
                ResourceId    = Resource.ResourceItem,
                Name          = "Export Resource",
                Description   = "Allows exporting resource data"
            },

            #endregion Resource

            #region Role

            new Permission
            {
                Id            = 11,
                ActionId  = PermissionAction.Create,
                ResourceId    = Resource.Role,
                Name          = "Create Role",
                Description   = "Allows creating roles"
            },
            new Permission
            {
                Id            = 12,
                ActionId  = PermissionAction.Read,
                ResourceId    = Resource.Role,
                Name          = "Read Role",
                Description   = "Allows reading roles"
            },
            new Permission
            {
                Id            = 13,
                ActionId  = PermissionAction.Update,
                ResourceId    = Resource.Role,
                Name          = "Update Role",
                Description   = "Allows updating roles"
            },
            new Permission
            {
                Id            = 14,
                ActionId  = PermissionAction.Delete,
                ResourceId    = Resource.Role,
                Name          = "Delete Role",
                Description   = "Allows deleting roles"
            },
            new Permission
            {
                Id            = 15,
                ActionId  = PermissionAction.Export,
                ResourceId    = Resource.Role,
                Name          = "Export Role",
                Description   = "Allows exporting role data"
            },

            #endregion Role

            #region Permission

            new Permission
            {
                Id            = 16,
                ActionId  = PermissionAction.Create,
                ResourceId    = Resource.Permission,
                Name          = "Create Permission",
                Description   = "Allows creating permissions"
            },
            new Permission
            {
                Id            = 17,
                ActionId  = PermissionAction.Read,
                ResourceId    = Resource.Permission,
                Name          = "Read Permission",
                Description   = "Allows reading permissions"
            },
            new Permission
            {
                Id            = 18,
                ActionId  = PermissionAction.Update,
                ResourceId    = Resource.Permission,
                Name          = "Update Permission",
                Description   = "Allows updating permissions"
            },
            new Permission
            {
                Id            = 19,
                ActionId  = PermissionAction.Delete,
                ResourceId    = Resource.Permission,
                Name          = "Delete Permission",
                Description   = "Allows deleting permissions"
            },
            new Permission
            {
                Id            = 20,
                ActionId  = PermissionAction.Export,
                ResourceId    = Resource.Permission,
                Name          = "Export Permission",
                Description   = "Allows exporting permission data"
            },

            #endregion Permission

            #region Action

            new Permission
            {
                Id            = 21,
                ActionId  = PermissionAction.Read,
                ResourceId    = Resource.Action,
                Name          = "Read Action",
                Description   = "Allows reading actions"
            },

            #endregion Action

            #region Category

            new Permission
            {
                Id            = 22,
                ActionId  = PermissionAction.Create,
                ResourceId    = Resource.Category,
                Name          = "Create Category",
                Description   = "Allows creating categories"
            },
            new Permission
            {
                Id            = 23,
                ActionId  = PermissionAction.Read,
                ResourceId    = Resource.Category,
                Name          = "Read Category",
                Description   = "Allows reading categories"
            },
            new Permission
            {
                Id            = 24,
                ActionId  = PermissionAction.Update,
                ResourceId    = Resource.Category,
                Name          = "Update Category",
                Description   = "Allows updating categories"
            },
            new Permission
            {
                Id            = 25,
                ActionId  = PermissionAction.Delete,
                ResourceId    = Resource.Category,
                Name          = "Delete Category",
                Description   = "Allows deleting categories"
            },
            new Permission
            {
                Id            = 26,
                ActionId  = PermissionAction.Export,
                ResourceId    = Resource.Category,
                Name          = "Export Category",
                Description   = "Allows exporting category data"
            },

            #endregion Category

            #region Item

            new Permission
            {
                Id            = 27,
                ActionId  = PermissionAction.Create,
                ResourceId    = Resource.Item,
                Name          = "Create Item",
                Description   = "Allows creating items"
            },
            new Permission
            {
                Id            = 28,
                ActionId  = PermissionAction.Read,
                ResourceId    = Resource.Item,
                Name          = "Read Item",
                Description   = "Allows reading items"
            },
            new Permission
            {
                Id            = 29,
                ActionId  = PermissionAction.Update,
                ResourceId    = Resource.Item,
                Name          = "Update Item",
                Description   = "Allows updating items"
            },
            new Permission
            {
                Id            = 30,
                ActionId  = PermissionAction.Delete,
                ResourceId    = Resource.Item,
                Name          = "Delete Item",
                Description   = "Allows deleting items"
            },
            new Permission
            {
                Id            = 31,
                ActionId  = PermissionAction.Export,
                ResourceId    = Resource.Item,
                Name          = "Export Item",
                Description   = "Allows exporting item data"
            },

            #endregion Item

            #region Configuration

            new Permission
            {
                Id            = 32,
                ActionId  = PermissionAction.Read,
                ResourceId    = Resource.Configuration,
                Name          = "Read Configuration",
                Description   = "Allows reading configuration settings"
            }

            #endregion Configuration
        };
    }
}