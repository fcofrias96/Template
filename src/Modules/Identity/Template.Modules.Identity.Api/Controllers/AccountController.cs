using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Web.Extensions;
using Template.Modules.Identity.Application.Contracts.Services;

namespace Template.Modules.Identity.Api.Controllers
{
    [Route("api/v{version:apiVersion}/accounts")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> RegisterAsync(
            [FromBody] UserCreateDto userCreateDTO,
            CancellationToken cancellationToken)
        {
            var result = await _authService.CreateAsync(userCreateDTO, cancellationToken);
            return result.Match(response => Ok(response));
        }
    }
}
