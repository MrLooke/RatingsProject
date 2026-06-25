namespace MediaBoard.Server.Features.Authentication
{
    public class RegisterResult
    {
        public int UserId { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }
    }
}
