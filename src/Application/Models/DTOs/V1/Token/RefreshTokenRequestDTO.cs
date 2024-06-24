namespace Template.Application.Models.DTOs.V1.Token
{
    public class RefreshTokenRequestDTO
    {
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}