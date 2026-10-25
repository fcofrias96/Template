using System.Security.Claims;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Application.Dtos.Menu;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IPermissionService _permissionService;
        private readonly IMenuRepository _menuRepository;

        public MenuService(IPermissionService permissionService, IMenuRepository menuRepository)
        {
            _permissionService = permissionService;
            _menuRepository = menuRepository;
        }

        public async Task<ApiResponse<MenuItemResponse>> GetMenuForUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            var userPermissionIds = await _permissionService.GetUserPermissionIdsAsync(user, cancellationToken);

            var menuItems = await _menuRepository.GetMenuItemsAsync(cancellationToken);

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

        private static List<MenuItem> FilterMenuItemsByPermissions(List<MenuItem> menuItems, HashSet<int> userPermissionIds)
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
