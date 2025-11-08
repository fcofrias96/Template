using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.Token;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Settings;

namespace Template.Api.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServiceAsync _authService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IAuthServiceAsync authService, IOptions<JwtSettings> jwtSettings)
        {
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthenticationResponseDto>>> LoginAsync(LoginDto loginDTO, CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(loginDTO, cancellationToken);
            var cookieOptions = GetCookieOptions();
            Response.Cookies.Append("refreshToken", response!.Data!.RefreshToken, cookieOptions);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> RegisterAsync([FromBody] UserCreateDto userCreateDTO, CancellationToken cancellationToken)
        {
            var user = await _authService.CreateAsync(userCreateDTO, cancellationToken);
            return Ok(user);
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult<ApiResponse>> ConfirmEmailAsync([FromQuery] ConfirmEmailDto confirmEmailDTO, CancellationToken cancellationToken )
        {
            var confirmaEmail = await _authService.ConfirmEmailAsync(confirmEmailDTO, cancellationToken);
            return Ok(confirmaEmail);
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<ActionResult<ApiResponse>> ResendConfirmationEmailAsync([FromBody] ResendConfirmationEmailDto resendConfirmationEmail, CancellationToken cancellationToken)
        {
            var confirmaEmail = await _authService.ResendConfirmationEmailAsync(resendConfirmationEmail, cancellationToken);
            return Ok(confirmaEmail);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDTO, CancellationToken cancellationToken)
        {
            var response = await _authService.ForgotPasswordAsync(forgotPasswordDTO, cancellationToken);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPasswordAsync(ResetPasswordDto resetPasswordDTO, CancellationToken cancellationToken)
        {
            var resetPassword = await _authService.ResetPasswordAsync(resetPasswordDTO, cancellationToken);
            return Ok(resetPassword);
        }

        [Authorize]
        [HttpPost("change-email")]
        public async Task<ActionResult<ApiResponse>> ChangeEmailAsync([FromBody] ChangeEmailDto changeEmailDto, CancellationToken cancellationToken)
        {
            var response = await _authService.ChangeEmailAsync(changeEmailDto, cancellationToken);
            return Ok(response);
        }

        [HttpGet("confirm-change-email")]
        public async Task<ActionResult<ApiResponse>> ConfirmChangeEmailAsync([FromQuery] ConfirmChangeEmailDto confirmChangeEmailDto, CancellationToken cancellationToken)
        {
            var response = await _authService.ConfirmChangeEmailAsync(confirmChangeEmailDto, cancellationToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);
            var cookieOptions = GetCookieOptions();
            Response.Cookies.Append("refreshToken", response!.Data!.RefreshToken, cookieOptions);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> LogoutAsync(CancellationToken cancellationToken)
        {
            Response.Cookies.Delete("refreshToken");
            var responese = await _authService.LogoutAsync(cancellationToken);
            return Ok(responese);
        }

        private CookieOptions GetCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays)
            };
        }
    }
}