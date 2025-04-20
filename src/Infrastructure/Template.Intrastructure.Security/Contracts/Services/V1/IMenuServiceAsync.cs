using System.Security.Claims;
using Template.Application.Models.Wrappers;
using Template.Security.Dtos.Menu;

namespace Template.Security.Contracts.Services.V1
{
    public interface IMenuServiceAsync
    {
        Task<ApiResponse<MenuItemResponse>> GetMenuForUser(ClaimsPrincipal user);
    }
}