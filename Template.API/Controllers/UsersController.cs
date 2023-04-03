using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Template.Core.Application.DTOs.User;
using Template.Core.Domain.Constants;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Identity.Contracts.Services;
using Template.Infrastructure.Identity.Entity;

namespace Template.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        #region Private Variable

        private readonly IUserServiceAsync _userServiceAsync;

        #endregion Private Variable

        #region Constructor

        public UsersController(IUserServiceAsync userService)
        {
            _userServiceAsync = userService;
        }

        #endregion Constructor

        #region Actions

        // POST api/users/{id}/roles
        [Authorize(Permission.Create, Roles = Role.Admin)]
        [HttpPost("{id}/permissions")]
        public async Task<ActionResult> AddPermissionsAsync(Guid id, [FromBody] List<string> permissions)
        {
            await _userServiceAsync.AddPermissionsAsync(id, permissions);
            return NoContent();
        }

        // GET api/users/{id}/roles
        [Authorize(Permission.Create, Roles = Role.Admin)]
        [HttpGet("{id}/permissions")]
        public async Task<ActionResult<IList<Claim>>> GetPermissionsAsync(Guid id)
        {
            var permissions = await _userServiceAsync.GetPermissionsAsync(id);
            return Ok(permissions);
        }

        // POST api/users/{id}/roles
        [Authorize(Permission.Create, Roles = Role.Admin)]
        [HttpPost("{id}/roles")]
        public async Task<ActionResult> AddRolesAsync(Guid id, [FromBody] List<string> roles)
        {
            await _userServiceAsync.AddToRolesAsync(id, roles);
            return NoContent();
        }

        // POST api/users/
        [AllowAnonymous]
        [HttpPost]
        public async Task<string> CreateAsync([FromBody] UserCreateDTO userCreateDTO)
        {
            return await _userServiceAsync.AddAsync(userCreateDTO);
        }

        // DELETE api/users/{id}
        [Authorize(Permission.Delete, Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _userServiceAsync.DeleteAsync(id);
            return NoContent();
        }

        // GET api/users/
        [Authorize(Permission.Read, Roles = Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserListDTO>>> GetAllUsers([FromQuery] QueryParameters<User> queryParameters)
        {
            var pagedUsersResult = await _userServiceAsync.GetAllAsync(queryParameters);
            return Ok(pagedUsersResult);
        }

        // GET api/users/{id}
        [Authorize(Permission.Read, Roles = Role.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailDTO>> GetAsync(Guid id)
        {
            var user = await _userServiceAsync.GetByIdAsync(id);
            return Ok(user);
        }

        // GET api/users/{id}/roles
        [Authorize(Permission.Read, Roles = Role.Admin)]
        [HttpGet("{id}/roles")]
        public async Task<ActionResult<IList<string>>> GetRolesAsync(Guid id)
        {
            var roles = await _userServiceAsync.GetRolesAsync(id);
            return Ok(roles);
        }

        // POST api/users/{id}/roles
        [Authorize(Permission.Delete, Roles = Role.Admin)]
        [HttpDelete("{id}/roles")]
        public async Task<ActionResult> RemoveRolesAsync(Guid id, [FromBody] List<string> roles)
        {
            await _userServiceAsync.RemoveFromRolesAsync(id, roles);
            return NoContent();
        }

        // PUT api/users/update/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            await _userServiceAsync.UpdateAsync(id, userUpdateDTO);
            return NoContent();
        }

        #endregion Actions
    }
}