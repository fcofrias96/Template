namespace Template.Application.Models.DTOs.V1.Users
{
    public class UserListDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}