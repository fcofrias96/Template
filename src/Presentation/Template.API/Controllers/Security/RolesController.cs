using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Api.Controllers.Common;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.Specification;
using Template.Application.Models.Wrappers;
using Template.Domain.Entities.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Permissions.Template.Application.Models.DTOs.V1.Roles;
using Template.Security.Dtos.RolePermission;
using Template.Security.Dtos.Roles;
using Template.Security.Entities;




namespace Template.Api.Controllers.Security
    {
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiController]
        [ApiVersion("1.0")]
        public class RolesController : ApiControllerBase
        {
            private readonly IRoleService _roleService;

            public RolesController(IRoleService roleService)
            {
                _roleService = roleService;
            }

            [Permission(Resource.Role, PermissionAction.Read)]
            [HttpGet]
            public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleListDto>>>> GetProjectedPaginatedAsync(
                [FromQuery] RoleQueryParameters request,
                CancellationToken cancellationToken)
            {
                var specification = new Specification<ApplicationRole>(request);
                var roles = await _roleService.GetProjectedPaginatedAsync<RoleListDto>(specification, cancellationToken);
                return Ok(roles);
            }

            [Permission(Resource.Role, PermissionAction.Read)]
            [HttpGet("{id}")]
            public async Task<ActionResult<ApiResponse<RoleDetailDto>>> GetRole(
                int id,
                CancellationToken cancellationToken)
            {
                var role = await _roleService.GetByIdProjectedAsync<int, RoleDetailDto>(id, cancellationToken);
                return Ok(role);
            }

            [Permission(Resource.Role, PermissionAction.Create)]
            [HttpPost]
            public async Task<ActionResult> CreateRole(
                [FromBody] RoleCreateDto roleCreateDto,
                CancellationToken cancellationToken)
            {
                var role = await _roleService.CreateAsync(roleCreateDto, cancellationToken);
                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }

            [Permission(Resource.Role, PermissionAction.Update)]
            [HttpPatch("{id}")]
            public async Task<ActionResult> UpdateRole(
                int id,
                [FromBody] RoleUpdateDto roleUpdateDTO,
                CancellationToken cancellationToken)
            {
                await _roleService.UpdateAsync(id.ToString(), roleUpdateDTO, cancellationToken);
                return NoContent();
            }

            [Permission(Resource.Role, PermissionAction.Delete)]
            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteRole(int id, CancellationToken cancellationToken)
            {
                await _roleService.SoftDeleteAsync(id, cancellationToken);
                return NoContent();
            }

            [Permission(Resource.Role, PermissionAction.Read)]
            [HttpGet("catalog")]
            public async Task<ActionResult<ApiResponse<IReadOnlyList<Catalog>>>> GetProjectedAsync(
                CancellationToken cancellationToken)
            {
                var result = await _roleService.GetProjectedWithEtagAsync<Catalog>(cancellationToken);
                return FromEtagResult(result);
            }


            [Permission(Resource.Role, PermissionAction.Read)]
            [HttpGet("{id}/permissions")]
            public async Task<ActionResult<ApiResponse<IReadOnlyList<RolePermissionDto>>>> GetRolePermissions(
                int id,
                CancellationToken cancellationToken)
            {
                var result = await _roleService.GetPermissionsAsync(id, cancellationToken);
                return Ok(result);
            }

            [Permission(Resource.Role, PermissionAction.Update)]
            [HttpPatch("{id}/permissions")]
            public async Task<ActionResult> UpdateRolePermissions(
                int id,
                [FromBody] UpdateRolePermissionsDto request,
                CancellationToken cancellationToken)
            {
                await _roleService.UpdatePermissionsAsync(id, request.Permissions, cancellationToken);
                return NoContent();
            }

            
        }
    }

