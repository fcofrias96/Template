using Microsoft.AspNetCore.Mvc;
using Template.Core.Application.DTOs.Auth;
using Template.Core.Application.Wrappers;
using Template.Infrastructure.Identity.Contracts.Services;

namespace Template.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServiceAsync _authService;
        public AuthController(IAuthServiceAsync authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<Response<AuthenticationResponseDTO>>> Login(LoginDTO loginDTO)
        {
            var response = await _authService.LoginAsync(loginDTO, GenerateIPAddress());
            return Ok(response);
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult<Response<string>>> ConfirmEmailAsync([FromQuery] ConfirmEmailDTO confirmEmailDTO)
        {
            var confirmaEmail = await _authService.ConfirmEmailAsync(confirmEmailDTO);
            return Ok(confirmaEmail);
        }
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            await _authService.ForgotPassword(forgotPasswordDTO);
            return Ok();
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult<Response<string>>> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var resetPassword = await _authService.ResetPassword(resetPasswordDTO);
            return Ok(resetPassword);
        }

        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
