using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Contracts.Authorization;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Application.Models.Specification;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Dtos.UserRole;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Api.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userServiceAsync;

        public UsersController(IUserService userService)
        {
            _userServiceAsync = userService;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _userServiceAsync.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedCollection<UserListDto>>>> GetAllUsers(
            [FromQuery] UserQueryParameters request,
            CancellationToken cancellationToken)
        {
            var specification = new Specification<ApplicationUser>(request);
            var pagedUsersResult = await _userServiceAsync
                .GetProjectedPaginatedAsync<UserListDto>(specification, cancellationToken);

            return Ok(pagedUsersResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var user = await _userServiceAsync.GetDetailAsync(id, cancellationToken);
            return Ok(user);
        }

        [HttpGet("{id}/roles")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<string>>>> GetRolesAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var response = await _userServiceAsync.GetRolesAsync(id, cancellationToken);
            return Ok(response);
        }

        [HttpPut("{id}/roles")]
        public async Task<ActionResult> UpdateRolesAsync(
            int id,
            [FromBody] UpdateUserRolesDto request,
            CancellationToken cancellationToken)
        {
            await _userServiceAsync.UpdateUserRolesAsync(id, request.RoleIds, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateAsync(
            int id,
            [FromBody] UserUpdateDto userUpdateDTO,
            CancellationToken cancellationToken)
        {
            await _userServiceAsync.UpdateAsync(id, userUpdateDTO, cancellationToken);
            return NoContent();
        }
    }
}
