using MediaBoard.Server.Exceptions;
using MediaBoard.Server.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MediaBoard.Server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IAuthService authService, IWebHostEnvironment env, IOptions<JwtSettings> jwtSettings)
        {
            _authService = authService;
            _env = env;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            RegisterResult user = await _authService.RegisterUserAsync(request);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResult user = await _authService.LoginUserAsync(request);

            Response.Cookies.Append("access_token", user.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessExpiryMinutes)
            });

            Response.Cookies.Append("refresh_token", user.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshExpiryDays)
            });

            return Ok(new { user.UserId, user.Username, user.Email });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if(Request.Cookies.TryGetValue("refresh_token", out var refreshToken)) {
                RefreshResult result = await _authService.RefreshAsync(refreshToken);

                Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !_env.IsDevelopment(),
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshExpiryDays)
                });

                Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !_env.IsDevelopment(),
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessExpiryMinutes)
                });
            }
            else
            {
                throw new UnauthorizedException("Refresh token not found.");
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                await _authService.LogoutAsync(refreshToken);
            }

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            UserCheckResult user = new UserCheckResult
            { 
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("Expected claim type id is null.")),
                Username = User.FindFirstValue(ClaimTypes.Name) ?? throw new UnauthorizedException("Expected claim type username is null."),
                Email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Expected claim type email is null.")
            };

            return Ok(user);
        }
    }
}
