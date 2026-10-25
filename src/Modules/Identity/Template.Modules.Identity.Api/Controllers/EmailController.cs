using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Application.Dtos.Auth;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;

namespace Template.Modules.Identity.Api.Controllers
{
    [Route("api/v{version:apiVersion}/email")]
    [ApiController]
    [ApiVersion("1.0")]
    public class EmailController : ControllerBase
    {
        private readonly IAuthService _authService;

        public EmailController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("confirm")]
        public async Task<ActionResult<ApiResponse>> ConfirmEmailAsync(
            [FromQuery] ConfirmEmailDto confirmEmailDTO,
            CancellationToken cancellationToken)
        {
            var response = await _authService.ConfirmEmailAsync(confirmEmailDTO, cancellationToken);
            return Ok(response);
        }

        [HttpPost("resend-confirmation")]
        public async Task<ActionResult<ApiResponse>> ResendConfirmationEmailAsync(
            [FromBody] ResendConfirmationEmailDto resendConfirmationEmail,
            CancellationToken cancellationToken)
        {
            var response = await _authService.ResendConfirmationEmailAsync(resendConfirmationEmail, cancellationToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("change")]
        public async Task<ActionResult<ApiResponse>> ChangeEmailAsync(
            [FromBody] ChangeEmailDto changeEmailDto,
            CancellationToken cancellationToken)
        {
            var response = await _authService.ChangeEmailAsync(changeEmailDto, cancellationToken);
            return Ok(response);
        }

        [HttpGet("confirm-change")]
        public async Task<ActionResult<ApiResponse>> ConfirmChangeEmailAsync(
            [FromQuery] ConfirmChangeEmailDto confirmChangeEmailDto,
            CancellationToken cancellationToken)
        {
            var response = await _authService.ConfirmChangeEmailAsync(confirmChangeEmailDto, cancellationToken);
            return Ok(response);
        }
    }
}
