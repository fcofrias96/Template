using System.Text.Json.Serialization;
using Template.Application.Models.DTOs.V1.Resource;

namespace Template.Application.Models.DTOs.V1.Auth
{
    public class AuthenticationResponseDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<ResourcePermissionDto> Modules { get; set; }
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}