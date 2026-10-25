using Microsoft.AspNetCore.Mvc;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Dtos.Menu;

namespace Template.Modules.Identity.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<MenuItemResponse>>> GetMenusAsync(CancellationToken cancellationToken)
        {
            var response = await _menuService.GetMenuForUserAsync(User, cancellationToken);
            return Ok(response);
        }
    }
}
