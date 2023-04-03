using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Core.Application.DTOs.Role;
using Template.Core.Domain.Constants;
using Template.Infrastructure.Identity.Contracts.Services;

namespace Template.Api.Controllers
{
    [Authorize(Roles = Role.Admin)]
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
        [CacheControl]
        public async Task<ActionResult<List<RoleListDTO>>> GetAllAsync()
        {
            var (roles, eTag) = await _roleService.GetAllWithETagAsync();
            Response.Headers.Add("X-Validate-ETag", eTag);
            return Ok(roles);
        }
    }
}