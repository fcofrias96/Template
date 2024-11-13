using System.Security.Claims;
using Template.Security.Dtos.Menu;

namespace Template.Security.Contracts.Services.V1
{
    public interface IMenuServiceAsync
    {
        Task<List<MenuItemDto>> GetMenuForUser(ClaimsPrincipal user);
    }
}