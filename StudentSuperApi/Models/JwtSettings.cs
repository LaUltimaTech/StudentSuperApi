namespace StudentSuperApi.Models
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpiresMinutes { get; set; } = 60;
    }
}

