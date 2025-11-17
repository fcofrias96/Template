using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.Token;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Settings;

namespace Template.Api.Controllers.Security
{
    /// <summary>
    /// Handles user authentication operations such as login, token refresh, and logout.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints related to authentication using JWT tokens.
    /// It manages access and refresh tokens through secure HTTP-only cookies.
    /// </remarks>
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthServiceAsync _authService;
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="authService">Service responsible for authentication logic.</param>
        /// <param name="jwtSettings">JWT configuration settings (token lifetime, etc.).</param>
        public AuthenticationController(IAuthServiceAsync authService, IOptions<JwtSettings> jwtSettings)
        {
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Authenticates the user with provided credentials and returns a JWT token.
        /// </summary>
        /// <param name="loginDTO">User credentials (username and password).</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>
        /// Returns an <see cref="ApiResponse{T}"/> containing an <see cref="AuthenticationResponseDto"/> 
        /// with access and refresh tokens if the authentication is successful.
        /// </returns>
        /// <response code="200">Returns a valid authentication response.</response>
        /// <response code="400">If the request body is invalid or missing fields.</response>
        /// <response code="401">If the credentials are incorrect.</response>
        /// <response code="500">If an unexpected error occurs during login.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthenticationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<AuthenticationResponseDto>>> LoginAsync(
            [FromBody] LoginDto loginDTO,
            CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(loginDTO, cancellationToken);

            var cookieOptions = GetCookieOptions();
            Response.Cookies.Append("refreshToken", response!.Data!.RefreshToken, cookieOptions);

            return Ok(response);
        }

        /// <summary>
        /// Generates a new access token using the refresh token stored in cookies.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>
        /// Returns an <see cref="ApiResponse{T}"/> containing a new 
        /// <see cref="RefreshTokenResponseDto"/> with refreshed tokens.
        /// </returns>
        /// <response code="200">Returns a new access and refresh token.</response>
        /// <response code="400">If the refresh token is missing or invalid.</response>
        /// <response code="401">If the refresh token has expired or is revoked.</response>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshTokenAsync(
            CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);

            var cookieOptions = GetCookieOptions();
            Response.Cookies.Append("refreshToken", response!.Data!.RefreshToken, cookieOptions);

            return Ok(response);
        }

        /// <summary>
        /// Logs out the currently authenticated user by invalidating their refresh token.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>
        /// Returns a generic <see cref="ApiResponse"/> confirming successful logout.
        /// </returns>
        /// <response code="200">Logout successful.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> LogoutAsync(CancellationToken cancellationToken)
        {
            Response.Cookies.Delete("refreshToken");
            var response = await _authService.LogoutAsync(cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Returns default cookie options for storing secure refresh tokens.
        /// </summary>
        /// <remarks>
        /// Cookies are configured as <c>HttpOnly</c> and <c>Secure</c> for safety, 
        /// with a lifetime equal to the refresh token duration defined in <see cref="JwtSettings"/>.
        /// </remarks>
        private CookieOptions GetCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays)
            };
        }
    }
}
