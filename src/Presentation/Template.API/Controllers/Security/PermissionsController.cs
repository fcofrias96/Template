using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Api.Controllers.Common;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Permissions;
using Template.Application.Models.Specification;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Permissions;
using Template.Security.Entities;
using ActionType = Template.Application.Enums.ActionType;

namespace Template.Api.Controllers.Security
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PermissionsController :  ApiControllerBase
    {
        private readonly IPermissionServiceAsync _permissionService;

        public PermissionsController(IPermissionServiceAsync permissionService)
        {
            _permissionService = permissionService;
        }

        [Permission(Resources.Permission, ActionType.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<PermissionListDto>>>> GetPermissions([FromQuery] PermissionQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Permission>(request);
            var permissions = await _permissionService.GetProjectedPaginatedAsync<PermissionListDto>(specification, cancellationToken);
            return Ok(permissions);
        }

        [Permission(Resources.Permission, ActionType.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PermissionDetailDto>>> GetPermission(int id, CancellationToken cancellationToken)
        {
            var permission = await _permissionService.GetByIdProjectedAsync<int, PermissionDetailDto>(id, cancellationToken);
            return Ok(permission);
        }

        [Permission(Resources.Permission, ActionType.Create)]
        [HttpPost]
        public async Task<ActionResult> CreatePermission([FromBody] PermissionCreateDto permissionCreateDto, CancellationToken cancellationToken)
        {
            var permission = await _permissionService.CreateAsync(permissionCreateDto, cancellationToken);

            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permission);
        }

        [Permission(Resources.Permission, ActionType.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePermission(int id, [FromBody] PermissionUpdateDto permissionUpdateDTO, CancellationToken cancellationToken)
        {
            await _permissionService.UpdateAsync(id, permissionUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Permission, ActionType.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePermission(int id, CancellationToken cancellationToken)
        {
            await _permissionService.SoftDeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Permission, ActionType.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<Catalog>>>> GetProjectedAsync(CancellationToken cancellationToken)
        {
			var result = await _permissionService.GetProjectedWithEtagAsync<Catalog>(cancellationToken);
			return FromEtagResult(result);
		}
    }
}