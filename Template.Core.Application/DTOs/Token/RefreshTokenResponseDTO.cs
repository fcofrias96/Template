using System.Text.Json.Serialization;

namespace Template.Core.Application.DTOs.Token
{
    public class RefreshTokenResponseDTO
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}