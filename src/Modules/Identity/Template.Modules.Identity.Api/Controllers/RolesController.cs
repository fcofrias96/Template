using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Contracts.Authorization;
using Template.BuildingBlocks.Web.Extensions;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Identity.Application.Dtos.Lookups;
using Template.Modules.Identity.Application.Dtos.Roles;
using Template.BuildingBlocks.Application.Models.Specification;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Dtos.RolePermission;
using Template.Modules.Identity.Domain.Entities;




namespace Template.Modules.Identity.Api.Controllers
    {
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiController]
        [ApiVersion("1.0")]
        public class RolesController : ControllerBase
        {
            private readonly IRoleService _roleService;

            public RolesController(IRoleService roleService)
            {
                _roleService = roleService;
            }

            [Permission(PermissionResources.Role, PermissionActions.Read)]
            [HttpGet]
            public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleListDto>>>> GetProjectedPaginatedAsync(
                [FromQuery] RoleQueryParameters request,
                CancellationToken cancellationToken)
            {
                var specification = new Specification<ApplicationRole>(request);
                var roles = await _roleService.GetProjectedPaginatedAsync<RoleListDto>(specification, cancellationToken);
                return Ok(roles);
            }

            [Permission(PermissionResources.Role, PermissionActions.Read)]
            [HttpGet("{id}")]
            public async Task<ActionResult<ApiResponse<RoleDetailDto>>> GetRole(
                int id,
                CancellationToken cancellationToken)
            {
                var role = await _roleService.GetByIdProjectedAsync<RoleDetailDto>(id, cancellationToken);
                return Ok(role);
            }

            [Permission(PermissionResources.Role, PermissionActions.Create)]
            [HttpPost]
            public async Task<ActionResult> CreateRole(
                [FromBody] RoleCreateDto roleCreateDto,
                CancellationToken cancellationToken)
            {
                var role = await _roleService.CreateAsync(roleCreateDto, cancellationToken);
                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }

            [Permission(PermissionResources.Role, PermissionActions.Update)]
            [HttpPut("{id}")]
            public async Task<ActionResult> UpdateRole(
                int id,
                [FromBody] RoleUpdateDto roleUpdateDTO,
                CancellationToken cancellationToken)
            {
                await _roleService.UpdateAsync(id, roleUpdateDTO, cancellationToken);
                return NoContent();
            }

            [Permission(PermissionResources.Role, PermissionActions.Delete)]
            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteRole(int id, CancellationToken cancellationToken)
            {
                await _roleService.SoftDeleteAsync(id, cancellationToken);
                return NoContent();
            }

            [Permission(PermissionResources.Role, PermissionActions.Read)]
            [HttpGet("catalog")]
            public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetProjectedAsync(
                CancellationToken cancellationToken)
            {
                var result = await _roleService.GetProjectedWithEtagAsync<RoleLookupDto>(cancellationToken);
                return result.ToActionResult();
            }


            [Permission(PermissionResources.Role, PermissionActions.Read)]
            [HttpGet("{id}/permissions")]
            public async Task<ActionResult<ApiResponse<IReadOnlyList<RolePermissionDto>>>> GetRolePermissions(
                int id,
                CancellationToken cancellationToken)
            {
                var result = await _roleService.GetPermissionsAsync(id, cancellationToken);
                return Ok(result);
            }

            [Permission(PermissionResources.Role, PermissionActions.Update)]
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

