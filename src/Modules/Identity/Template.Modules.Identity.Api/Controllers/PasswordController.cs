using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Application.Dtos.Auth;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;

namespace Template.Modules.Identity.Api.Controllers
{
    [Route("api/v{version:apiVersion}/password")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PasswordController : ControllerBase
    {
        private readonly IAuthService _authService;

        public PasswordController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("forgot")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse>> ForgotPasswordAsync(
            [FromBody] ForgotPasswordDto forgotPasswordDTO,
            CancellationToken cancellationToken)
        {
            var response = await _authService.ForgotPasswordAsync(forgotPasswordDTO, cancellationToken);
            return Ok(response);
        }

        [HttpPost("reset")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse>> ResetPasswordAsync(
            [FromBody] ResetPasswordDto resetPasswordDTO,
            CancellationToken cancellationToken)
        {
            var response = await _authService.ResetPasswordAsync(resetPasswordDTO, cancellationToken);
            return Ok(response);
        }
    }
}
