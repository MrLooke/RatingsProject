using MediaBoard.Server.Features.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MediaBoard.Server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
            return Ok(user);
        }
    }
}
