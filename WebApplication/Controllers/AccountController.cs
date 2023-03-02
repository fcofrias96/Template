using Microsoft.AspNetCore.Mvc;
using Template.Core.Application.DTOs.Auth;
using Template.Core.Application.DTOs.Token;
using Template.Core.Application.Interfaces.Services;
using ILogger = Serilog.ILogger;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        public AccountController(IAccountService accountService, ILogger logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(AuthenticationRequest request)
        {
            var response = await _accountService.LoginAsync(request);
            SetRefreshTokenInCookie(response.Data.RefreshToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDTO request)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            request.RefreshToken = refreshToken;

            var response = await _accountService.VerifyRefreshToken(request);
            if (response == null)
            {
                return BadRequest($"RefreshToken {refreshToken}");
            }
            SetRefreshTokenInCookie(response.Data?.RefreshToken);
            return Ok(response);
        }

        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(10),
                SameSite = SameSiteMode.None,
                Secure = true
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
            Response.Headers.AccessControlAllowCredentials = "true";
        }

        private string GenerateIpAdrress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }
    }
}