using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Template.Application.Models.Wrappers;
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

        public async Task<ApiResponse<MenuItemResponse>> GetMenuForUser(ClaimsPrincipal user)
        {
            var userPermissionIds = _permissionService.GetUserPermissionIds(user);

            var menuItems = await _context.MenuItems
                                    .Select(m => new MenuItem
                                    {
                                        Id = m.Id,
                                        Title = m.Title,
                                        Url = m.Url,
                                        Icon = m.Icon,
                                        ParentId = m.ParentId,
                                        PermissionId = m.PermissionId,
                                        Order = m.Order

                                    })
                                    .OrderBy(m => m.ParentId)
                                    .ThenBy(m=> m.Order)
                                    .ToListAsync();

            var accessibleMenuItems = FilterMenuItemsByPermissions(menuItems, userPermissionIds);

            var menuHierarchy = BuildMenuHierarchy(accessibleMenuItems);

            var menuDto = MapMenuItemsToDto(menuHierarchy);
            var menuResponse = new MenuItemResponse
            {
                Menus = menuDto,
                IsAuthenticated = user?.Identity?.IsAuthenticated ?? false,
            };

            return new ApiResponse<MenuItemResponse>(menuResponse);
        }



        private List<MenuItem> FilterMenuItemsByPermissions(List<MenuItem> menuItems, HashSet<int> userPermissionIds)
        {
            var accessibleMenuItems = menuItems
                .Where(item => HasAccessToMenuItem(userPermissionIds, item))
                .ToList();

            return accessibleMenuItems;
        }


        private static bool HasAccessToMenuItem(HashSet<int> userPermissionIds, MenuItem item)
        {
            if (!item.PermissionId.HasValue)
            {
                return true;
            }

            return userPermissionIds.Contains(item.PermissionId.Value);
        }


        private static List<MenuItem> BuildMenuHierarchy(List<MenuItem> menuItems)
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
                Children = item.Children != null ? MapMenuItemsToDto(item.Children.ToList()) : [],
                Order = item.Order
            }).ToList();
        }

    }

}