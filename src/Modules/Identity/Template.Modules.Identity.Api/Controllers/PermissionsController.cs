using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Contracts.Authorization;
using Template.BuildingBlocks.Web.Extensions;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Identity.Application.Dtos.Lookups;
using Template.Modules.Identity.Application.Dtos.Permissions;
using Template.BuildingBlocks.Application.Models.Specification;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Domain.Entities;


namespace Template.Modules.Identity.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [Permission(PermissionResources.Permission, PermissionActions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<PermissionListDto>>>> GetPermissions([FromQuery] PermissionQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Permission>(request);
            var permissions = await _permissionService.GetProjectedPaginatedAsync<PermissionListDto>(specification, cancellationToken);
            return Ok(permissions);
        }

        [Permission(PermissionResources.Permission, PermissionActions.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PermissionDetailDto>>> GetPermission(int id, CancellationToken cancellationToken)
        {
            var permission = await _permissionService.GetByIdProjectedAsync<PermissionDetailDto>(id, cancellationToken);
            return Ok(permission);
        }

        [Permission(PermissionResources.Permission, PermissionActions.Create)]
        [HttpPost]
        public async Task<ActionResult> CreatePermission([FromBody] PermissionCreateDto permissionCreateDto, CancellationToken cancellationToken)
        {
            var permission = await _permissionService.CreateAsync(permissionCreateDto, cancellationToken);

            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permission);
        }

        [Permission(PermissionResources.Permission, PermissionActions.Update)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdatePermission(int id, [FromBody] PermissionUpdateDto permissionUpdateDTO, CancellationToken cancellationToken)
        {
            await _permissionService.UpdateAsync(id, permissionUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(PermissionResources.Permission, PermissionActions.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePermission(int id, CancellationToken cancellationToken)
        {
            await _permissionService.SoftDeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [Permission(PermissionResources.Permission, PermissionActions.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetProjectedAsync(CancellationToken cancellationToken)
        {
			var result = await _permissionService.GetProjectedWithEtagAsync<PermissionLookupDto>(cancellationToken);
			return result.ToActionResult();
        }
    }
}
