using MediaBoard.Server.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MediaBoard.Server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;
        public AuthController(IAuthService authService, IWebHostEnvironment env)
        {
            _authService = authService;
            _env = env;
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
            string token = _authService.GenerateToken(user);
        

            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            return Ok(user);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            LoginResult user = new LoginResult { 
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException("Expected claim type id is null.")),
                Username = User.FindFirstValue(ClaimTypes.Name) ?? throw new ArgumentNullException("Expected claim type username is null."),
                Email = User.FindFirstValue(ClaimTypes.Email) ?? throw new ArgumentNullException("Expected claim type email is null.")
            };

            return Ok(user);
        }
    }
}
