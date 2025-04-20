using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Roles;
using Template.Security.Specification.V1;

namespace Template.Api.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize(Roles = Roles.Admin)]
    public class RolesController : ControllerBase
    {
        private readonly IRoleServiceAsync _roleService;
        private readonly IRolePermissionServiceAsync _rolePermissionService;

        public RolesController(IRoleServiceAsync roleService, IRolePermissionServiceAsync rolePermissionService)
        {
            _roleService = roleService;
            _rolePermissionService = rolePermissionService;
        }

        [Permission(Resources.Role, ActionType.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoleListDto>>>> GetProjectedPaginatedAsync([FromQuery]RoleQueryParameters request )
        {
            var roles = await _roleService.GetProjectedPaginatedAsync<RoleListDto, RoleSpecification>(request);
            return Ok(roles);
        }

        [Permission(Resources.Role, ActionType.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDetailDto>>> GetRole(Guid id)
        {
            var role = await _roleService.GetByIdProjectedAsync<Guid, RoleDetailDto>(id);
            return Ok(role);
        }

        [Permission(Resources.Role, ActionType.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] RoleCreateDto roleCreateDto)
        {
            var role = await _roleService.CreateAsync(roleCreateDto);

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        [Permission(Resources.Role, ActionType.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(Guid id, [FromBody] RoleUpdateDto roleUpdateDTO)
        {
            await _roleService.UpdateAsync(id.ToString(), roleUpdateDTO);
            return NoContent();
        }

        [Permission(Resources.Role, ActionType.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(Guid id)
        {
            await _roleService.SoftDeleteAsync(id);
            return NoContent();
        }

        [Permission(Resources.Role, ActionType.Read)]
        [HttpGet("dropdown")]
        public async Task<ActionResult<ApiResponse<List<Dropdown>>>> GetProjectedAsync()
        {
            var resources = await _roleService.GetProjectedAsync<Dropdown>();
            return Ok(resources);
        }

    }
}