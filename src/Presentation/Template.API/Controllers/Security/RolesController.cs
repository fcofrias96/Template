using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Roles;
using Template.Security.Specification.V1;

namespace Template.Api.Controllers.Security
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
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
        public async Task<ActionResult<ApiResponse<List<RoleListDto>>>> GetProjectedPaginatedAsync([FromQuery] RoleQueryParameters request, CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetProjectedPaginatedAsync<RoleListDto, RoleSpecification>(request, cancellationToken);
            return Ok(roles);
        }

        [Permission(Resources.Role, ActionType.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDetailDto>>> GetRole(Guid id, CancellationToken cancellationToken)
        {
            var role = await _roleService.GetByIdProjectedAsync<Guid, RoleDetailDto>(id, cancellationToken);
            return Ok(role);
        }

        [Permission(Resources.Role, ActionType.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] RoleCreateDto roleCreateDto, CancellationToken cancellationToken)
        {
            var role = await _roleService.CreateAsync(roleCreateDto, cancellationToken);

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        [Permission(Resources.Role, ActionType.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(Guid id, [FromBody] RoleUpdateDto roleUpdateDTO, CancellationToken cancellationToken)
        {
            await _roleService.UpdateAsync(id.ToString(), roleUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Role, ActionType.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
        {
            await _roleService.SoftDeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Role, ActionType.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<List<Catalog>>>> GetProjectedAsync(CancellationToken cancellationToken)
        {
            var resources = await _roleService.GetProjectedAsync<Catalog>(cancellationToken);
            return Ok(resources);
        }
    }
}