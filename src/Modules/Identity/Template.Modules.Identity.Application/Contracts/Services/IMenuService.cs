using System.Security.Claims;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Dtos.Menu;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IMenuService
    {
        Task<ApiResponse<MenuItemResponse>> GetMenuForUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
    }
}
