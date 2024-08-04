using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.Token;
using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Settings;

namespace Template.Api.Controllers
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
        public async Task<ActionResult<ApiResponse<AuthenticationResponseDTO>>> LoginAsync(LoginDto loginDTO)
        {
            var response = await _authService.LoginAsync(loginDTO);
            var cookieOptions = GetCookieOptions();
            Response.Cookies.Append("refreshToken", response!.Data!.RefreshToken, cookieOptions);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> RegisterAsync([FromBody] UserCreateDTO userCreateDTO)
        {
            var user = await _authService.CreateAsync(userCreateDTO);
            return Ok(user);
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult<ApiResponse>> ConfirmEmailAsync([FromQuery] ConfirmEmailDto confirmEmailDTO)
        {
            var confirmaEmail = await _authService.ConfirmEmailAsync(confirmEmailDTO);
            return Ok(confirmaEmail);
        }


        [HttpPost("resend-confirmation-email")]
        public async Task<ActionResult<ApiResponse>> ResendConfirmationEmailAsync([FromBody] ResendConfirmationEmailDto resendConfirmationEmail)
        {
            var confirmaEmail = await _authService.ResendConfirmationEmailAsync(resendConfirmationEmail);
            return Ok(confirmaEmail);
        }


        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDTO)
        {
            var response = await _authService.ForgotPassword(forgotPasswordDTO);
            return Ok(response);
        }


        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPasswordAsync(ResetPasswordDto resetPasswordDTO)
        {
            var resetPassword = await _authService.ResetPassword(resetPasswordDTO);
            return Ok(resetPassword);
        }



        [Authorize]
        [HttpPost("change-email")]
        public async Task<ActionResult<ApiResponse>> ChangeEmailAsync([FromBody] ChangeEmailDto changeEmailDto)
        {
            var response = await _authService.ChangeEmailAsync(changeEmailDto);
            return Ok(response);
        }

        [HttpGet("confirm-change-email")]
        public async Task<ActionResult<ApiResponse>> ConfirmChangeEmailAsync([FromQuery]ConfirmChangeEmailDto confirmChangeEmailDto)
        {

            var response = await _authService.ConfirmChangeEmailAsync(confirmChangeEmailDto);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authService.RefreshTokenAsync(refreshToken);
            var cookieOptions = GetCookieOptions();
            Response.Cookies.Append("refreshToken", response!.Data!.RefreshToken, cookieOptions);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> LogoutAsync()
        {
            Response.Cookies.Delete("refreshToken");
            var responese = await _authService.LogoutAsync();
            return Ok(responese);
        }


        private CookieOptions GetCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays)
            };
        }

    }
}