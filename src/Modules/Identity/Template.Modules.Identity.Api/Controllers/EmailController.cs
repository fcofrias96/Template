using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Application.Dtos.Auth;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Web.Extensions;
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
            var result = await _authService.ConfirmEmailAsync(confirmEmailDTO, cancellationToken);
            return result.Match(response => Ok(response));
        }

        [HttpPost("resend-confirmation")]
        public async Task<ActionResult<ApiResponse>> ResendConfirmationEmailAsync(
            [FromBody] ResendConfirmationEmailDto resendConfirmationEmail,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ResendConfirmationEmailAsync(resendConfirmationEmail, cancellationToken);
            return result.Match(response => Ok(response));
        }

        [Authorize]
        [HttpPost("change")]
        public async Task<ActionResult<ApiResponse>> ChangeEmailAsync(
            [FromBody] ChangeEmailDto changeEmailDto,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ChangeEmailAsync(changeEmailDto, cancellationToken);
            return result.Match(response => Ok(response));
        }

        [HttpGet("confirm-change")]
        public async Task<ActionResult<ApiResponse>> ConfirmChangeEmailAsync(
            [FromQuery] ConfirmChangeEmailDto confirmChangeEmailDto,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ConfirmChangeEmailAsync(confirmChangeEmailDto, cancellationToken);
            return result.Match(response => Ok(response));
        }
    }
}
