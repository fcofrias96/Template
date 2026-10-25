using System.Text.Json.Serialization;

namespace Template.Modules.Identity.Application.Dtos.Token
{
    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
