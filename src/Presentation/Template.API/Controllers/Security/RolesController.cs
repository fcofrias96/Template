using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Api.Controllers.Common;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.Specification;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Permissions.Template.Application.Models.DTOs.V1.Roles;
using Template.Security.Dtos.RolePermission;
using Template.Security.Dtos.Roles;
using Template.Security.Entities;
using ActionType = Template.Application.Enums.ActionType;



namespace Template.Api.Controllers.Security
    {
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiController]
        [ApiVersion("1.0")]
        public class RolesController : ApiControllerBase
        {
            private readonly IRoleServiceAsync _roleService;

            public RolesController(IRoleServiceAsync roleService)
            {
                _roleService = roleService;
            }

            [Permission(Resources.Role, ActionType.Read)]
            [HttpGet]
            public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleListDto>>>> GetProjectedPaginatedAsync(
                [FromQuery] RoleQueryParameters request,
                CancellationToken cancellationToken)
            {
                var specification = new Specification<Role>(request);
                var roles = await _roleService.GetProjectedPaginatedAsync<RoleListDto>(specification, cancellationToken);
                return Ok(roles);
            }

            [Permission(Resources.Role, ActionType.Read)]
            [HttpGet("{id}")]
            public async Task<ActionResult<ApiResponse<RoleDetailDto>>> GetRole(
                Guid id,
                CancellationToken cancellationToken)
            {
                var role = await _roleService.GetByIdProjectedAsync<Guid, RoleDetailDto>(id, cancellationToken);
                return Ok(role);
            }

            [Permission(Resources.Role, ActionType.Create)]
            [HttpPost]
            public async Task<ActionResult> CreateRole(
                [FromBody] RoleCreateDto roleCreateDto,
                CancellationToken cancellationToken)
            {
                var role = await _roleService.CreateAsync(roleCreateDto, cancellationToken);
                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }

            [Permission(Resources.Role, ActionType.Update)]
            [HttpPut("{id}")]
            public async Task<ActionResult> UpdateRole(
                Guid id,
                [FromBody] RoleUpdateDto roleUpdateDTO,
                CancellationToken cancellationToken)
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
            public async Task<ActionResult<ApiResponse<IReadOnlyList<Catalog>>>> GetProjectedAsync(
                CancellationToken cancellationToken)
            {
                var result = await _roleService.GetProjectedWithEtagAsync<Catalog>(cancellationToken);
                return FromEtagResult(result);
            }


            [Permission(Resources.Role, ActionType.Read)]
            [HttpGet("{id}/permissions")]
            public async Task<ActionResult<ApiResponse<IReadOnlyList<RolePermissionDto>>>> GetRolePermissions(
                Guid id,
                CancellationToken cancellationToken)
            {
                var result = await _roleService.GetPermissionsAsync(id.ToString(), cancellationToken);
                return Ok(result);
            }

            [Permission(Resources.Role, ActionType.Update)]
            [HttpPut("{id}/permissions")]
            public async Task<ActionResult> UpdateRolePermissions(
                Guid id,
                [FromBody] UpdateRolePermissionsDto request,
                CancellationToken cancellationToken)
            {
                await _roleService.UpdatePermissionsAsync(id.ToString(), request.Permissions, cancellationToken);
                return NoContent();
            }

            
        }
    }

