using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Dtos.Menu;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class MenuServiceAsync : IMenuServiceAsync
    {
        private readonly IPermissionService _permissionService;
        private readonly SecurityDbContext _context;

        public MenuServiceAsync(IPermissionService permissionService, SecurityDbContext context)
        {
            _permissionService = permissionService;
            _context = context;
        }

        public async Task<List<MenuItemDto>> GetMenuForUser(ClaimsPrincipal user)
        {
            var userResourceActionIds = _permissionService.GetUserResourceActionIds(user);

            var menuItems = await _context.MenuItems
                                    .Select(m => new MenuItem
                                    {
                                        Id = m.Id,
                                        Title = m.Title,
                                        Url = m.Url,
                                        Icon = m.Icon,
                                        ParentId = m.ParentId,
                                        ResourceActionId = m.ResourceActionId
                                    })
                                    .ToListAsync();

            var accessibleMenuItems = FilterMenuItemsByPermissions(menuItems, userResourceActionIds);

            var menuHierarchy = BuildMenuHierarchy(accessibleMenuItems);

            var menuDto = MapMenuItemsToDto(menuHierarchy);

            return menuDto;
        }



        private List<MenuItem> FilterMenuItemsByPermissions(List<MenuItem> menuItems, HashSet<int> userResourceActionIds)
        {
            var accessibleMenuItems = menuItems
                .Where(item => HasAccessToMenuItem(userResourceActionIds, item))
                .ToList();

            return accessibleMenuItems;
        }


        private bool HasAccessToMenuItem(HashSet<int> userResourceActionIds, MenuItem item)
        {
            if (!item.ResourceActionId.HasValue)
            {
                return true;
            }

            return userResourceActionIds.Contains(item.ResourceActionId.Value);
        }


        private List<MenuItem> BuildMenuHierarchy(List<MenuItem> menuItems)
        {
            var menuItemDictionary = menuItems.ToDictionary(item => item.Id);
            List<MenuItem> rootItems = new List<MenuItem>();

            foreach (var menuItem in menuItems)
            {
                if (menuItem.ParentId == null)
                {
                    rootItems.Add(menuItem);
                }
                else if (menuItemDictionary.ContainsKey(menuItem.ParentId.Value))
                {
                    var parent = menuItemDictionary[menuItem.ParentId.Value];
                    parent.Children.Add(menuItem);
                }
            }

            return rootItems;
        }

        private List<MenuItemDto> MapMenuItemsToDto(List<MenuItem> menuItems)
        {
            return menuItems.Select(item => new MenuItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Url = item.Url,
                Icon = item.Icon,
                Children = item.Children != null ? MapMenuItemsToDto(item.Children.ToList()) : null
            }).ToList();
        }

    }

}