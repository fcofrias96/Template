using Microsoft.AspNetCore.Mvc;
using Template.Api.Controllers.Common;
using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;

namespace Template.Api.Controllers.Security
{
    [Route("api/v{version:apiVersion}/password")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PasswordController :  ApiControllerBase
    {
        private readonly IAuthServiceAsync _authService;

        public PasswordController(IAuthServiceAsync authService)
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