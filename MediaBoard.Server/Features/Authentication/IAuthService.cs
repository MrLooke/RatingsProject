using MediaBoard.Server.Entities;

namespace MediaBoard.Server.Features.Authentication
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterUserAsync(RegisterRequest request);

        Task<LoginResult> LoginUserAsync(LoginRequest request);

        string GenerateToken(LoginResult user);
    }
}