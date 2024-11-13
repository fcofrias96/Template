using Template.Security.Entities;

namespace Template.Security.Seeds
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
                ResourceActionId = null
            },
            new MenuItem
            {
                Id = 2,
                Title = "Usuarios",
                Url = "/usuarios",
                Icon = "users-icon",
                ParentId = 6,
                ResourceActionId = 2
            },
            new MenuItem
            {
                Id = 3,
                Title = "Roles",
                Url = "/roles",
                Icon = "roles-icon",
                ParentId = 6,
                ResourceActionId = 12
            },
            new MenuItem
            {
                Id = 4,
                Title = "Categorías",
                Url = "/categorias",
                Icon = "categories-icon",
                ParentId = null,
                ResourceActionId = 23
            },
            new MenuItem
            {
                Id = 5,
                Title = "Productos",
                Url = "/productos",
                Icon = "items-icon",
                ParentId = null,
                ResourceActionId = 28
            },
            new MenuItem
            {
                Id = 6,
                Title = "Configuración",
                Url = "/configuracion",
                Icon = "settings-icon",
                ParentId = null,
                ResourceActionId = 32
            },
            new MenuItem
            {
                Id = 7,
                Title = "Permisos",
                Url = "/permisos",
                Icon = "permissions-icon",
                ParentId = 6,
                ResourceActionId = 17
            },
            new MenuItem
            {
                Id = 8,
                Title = "Recursos",
                Url = "/recursos",
                Icon = "resources-icon",
                ParentId = 6,
                ResourceActionId = 7
            }

        };
    }
}

