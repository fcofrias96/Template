using System.Text.Json.Serialization;
using Template.Application.Models.DTOs.V1.Resources;

namespace Template.Application.Models.DTOs.V1.Auth
{
    public class AuthenticationResponseDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IReadOnlyList<string> Roles { get; set; }
        public ResourcePermissionDto[] Resources { get; set; }
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}