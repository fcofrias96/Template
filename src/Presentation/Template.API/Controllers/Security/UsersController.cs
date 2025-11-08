using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Constants;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Dtos.Users;
using Template.Security.Specification.V1;

namespace Template.Api.Controllers.Admin
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        #region Private Variable

        private readonly IUserServiceAsync _userServiceAsync;

        #endregion Private Variable

        public UsersController(IUserServiceAsync userService)
        {
            _userServiceAsync = userService;
        }

        #region Actions

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _userServiceAsync.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedCollection<UserListDto>>>> GetAllUsers([FromQuery] UserQueryParameters request, CancellationToken cancellationToken)
        {
            var pagedUsersResult = await _userServiceAsync.GetProjectedPaginatedAsync<UserListDto, UserSpecification>(request, cancellationToken);
            return Ok(pagedUsersResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userServiceAsync.GetUserDetailAsync(id, cancellationToken);
            return Ok(user);
        }

        [HttpGet("{id}/roles")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetRolesAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _userServiceAsync.GetRolesAsync(id, cancellationToken);
            return Ok(response);
        }




        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UserUpdateDto userUpdateDTO, CancellationToken cancellationToken)
        {
            await _userServiceAsync.UpdateAsync(id, userUpdateDTO, cancellationToken);
            return NoContent();
        }

        #endregion Actions
    }
}