using System.Text.Json.Serialization;
using Template.Modules.Identity.Application.Dtos.Resources;

namespace Template.Modules.Identity.Application.Dtos.Auth
{
    public class AuthenticationResponseDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IReadOnlyList<string> Roles { get; set; }
        public ResourcePermissionDto[] Resources { get; set; }
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
