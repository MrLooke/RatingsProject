namespace MediaBoard.Server.Features.Authentication
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterUserAsync(RegisterRequest request);

        Task<LoginResult> LoginUserAsync(LoginRequest request);

        Task<RefreshResult> RefreshAsync(string refreshToken);

        Task LogoutAsync(string refreshToken);
    }
}