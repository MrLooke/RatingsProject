using System.ComponentModel.DataAnnotations;

namespace MediaBoard.Server.Features.Authentication
{
    public class LoginResult
    {
        public int UserId { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
