namespace MediaBoard.Server.Features.Authentication
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessExpiryMinutes { get; set; }
        public int RefreshExpiryDays { get; set; }
    }
}
