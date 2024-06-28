using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.Email;
using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;
using Template.Identity.Services.V1;

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
        public async Task<ActionResult<ApiResponse<AuthenticationResponseDTO>>> Login(LoginDto loginDTO)
        {
            var response = await _authService.LoginAsync(loginDTO);
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
        public async Task<ActionResult<ApiResponse>> ResendConfirmationEmail([FromBody] ResendConfirmationEmailDto resendConfirmationEmail)
        {
            var confirmaEmail = await _authService.ResendConfirmationEmailAsync(resendConfirmationEmail);
            return Ok(confirmaEmail);
        }




        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDTO)
        {
            var response = await _authService.ForgotPassword(forgotPasswordDTO);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword(ResetPasswordDto resetPasswordDTO)
        {
            var resetPassword = await _authService.ResetPassword(resetPasswordDTO);
            return Ok(resetPassword);
        }




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

    }
}