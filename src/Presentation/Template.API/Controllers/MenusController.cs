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
    public class MenusController : ControllerBase
    {
        private readonly IMenuServiceAsync _menuService;

        public MenusController(IMenuServiceAsync menuService)
        {
            _menuService = menuService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<MenuItemDto>>> GetMenusAsync()
        {
           
            var response = await  _menuService.GetMenuForUser(User);
            return Ok(response);
        }


    }
}