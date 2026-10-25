using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Seeds
{
    public static class MenuItemSeed
    {
        public static List<MenuItem> Data { get; } = new List<MenuItem>
        {
            new MenuItem
            {
                Id = 1,
                Title = "Dashboard",
                Url = "/dashboard",
                Icon = "dashboard-icon",
                ParentId = null,
                PermissionId = null,
                Order = 1,
            },
            new MenuItem
            {
                Id = 2,
                Title = "Usuarios",
                Url = "/usuarios",
                Icon = "users-icon",
                ParentId = 6,
                PermissionId = 2,
                Order = 2,
            },
            new MenuItem
            {
                Id = 3,
                Title = "Roles",
                Url = "/roles",
                Icon = "roles-icon",
                ParentId = 6,
                PermissionId = 12,
                Order = 1,
            },
            new MenuItem
            {
                Id = 4,
                Title = "Categorías",
                Url = "/categorias",
                Icon = "categories-icon",
                ParentId = null,
                PermissionId = 23,
                Order = 3
            },
            new MenuItem
            {
                Id = 5,
                Title = "Productos",
                Url = "/productos",
                Icon = "items-icon",
                ParentId = null,
                PermissionId = 28,
                Order = 2,
            },
            new MenuItem
            {
                Id = 6,
                Title = "Configuración",
                Url = "/configuracion",
                Icon = "settings-icon",
                ParentId = null,
                PermissionId = 32,
                Order = 4
            },
            new MenuItem
            {
                Id = 7,
                Title = "Permisos",
                Url = "/permisos",
                Icon = "permissions-icon",
                ParentId = 6,
                PermissionId = 17,
                Order = 3
            },
            new MenuItem
            {
                Id = 8,
                Title = "Recursos",
                Url = "/recursos",
                Icon = "resources-icon",
                ParentId = 6,
                PermissionId = 7,
                Order = 4,
            }
        };
    }
}
