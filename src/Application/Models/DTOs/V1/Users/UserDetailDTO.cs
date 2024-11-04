using System.Text.Json.Serialization;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;

namespace Template.Application.Models.DTOs.V1.Users
{
    public class UserDetailDto
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public RoleListDto[] Roles { get; set; }
        public bool IsVerified { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }

        public ResourcePermissionDto[] Resources { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}