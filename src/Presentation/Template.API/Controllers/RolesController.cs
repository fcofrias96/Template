using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Constants;
using Template.Application.Models.DTOs.V1.Role;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;

namespace Template.Api.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleServiceAsync _roleService;

        public RolesController(IRoleServiceAsync roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoleListDTO>>>> GetAllAsync()
        {
            var response = await _roleService.GetAllAsync();
            return Ok(response);
        }
    }
}