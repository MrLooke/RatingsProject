namespace MediaBoard.Server.Features.Authentication
{
    public class UserCheckResult
    {
        public int UserId { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }
    }
}
