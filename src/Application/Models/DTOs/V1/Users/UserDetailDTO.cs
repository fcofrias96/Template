using System.Text.Json.Serialization;
using Template.Application.Models.DTOs.V1.Resources;

namespace Template.Application.Models.DTOs.V1.Users
{
    public class UserDetailDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public bool IsVerified { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }

        public List<ResourcePermissionDto> ModulePermissions { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}