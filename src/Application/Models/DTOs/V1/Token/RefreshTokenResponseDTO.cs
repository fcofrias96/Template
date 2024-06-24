using System.Text.Json.Serialization;

namespace Template.Application.Models.DTOs.V1.Token
{
    public class RefreshTokenResponseDTO
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}