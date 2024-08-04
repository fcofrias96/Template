using System.Text.Json.Serialization;

namespace Template.Application.Models.DTOs.V1.Token
{
    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}