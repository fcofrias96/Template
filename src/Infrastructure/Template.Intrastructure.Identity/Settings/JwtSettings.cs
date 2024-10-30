namespace Template.Identity.Settings
{
    public class JwtSettings
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
        public int EmailConfirmationTokenDurationInHours { get; set; }
        public int PasswordResetTokenDurationInHours { get; set; }
        public int LoginTokenDurationInMinutes { get; set; }
        public int RefreshTokenDurationInDays { get; set; }
        public int ChangeEmailTokenDurationInHours { get; set; }
    }
}