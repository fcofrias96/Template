using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Menu;

namespace Template.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuServiceAsync _menuService;

        public MenuController(IMenuServiceAsync menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<MenuItemResponse>>> GetMenusAsync()
        {
            var response = await  _menuService.GetMenuForUser(User);
            return Ok(response);
        }


    }
}