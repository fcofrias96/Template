using Microsoft.AspNetCore.Mvc;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;

namespace Template.Api.Controllers.Account
{
    [Route("api/v{version:apiVersion}/accounts")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthServiceAsync _authService;

        public AccountController(IAuthServiceAsync authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> RegisterAsync(
            [FromBody] UserCreateDto userCreateDTO,
            CancellationToken cancellationToken)
        {
            var user = await _authService.CreateAsync(userCreateDTO, cancellationToken);
            return Ok(user);
        }
    }
}
