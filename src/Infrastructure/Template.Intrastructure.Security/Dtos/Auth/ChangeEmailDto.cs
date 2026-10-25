namespace Template.Application.Models.DTOs.V1.Auth
{
    public class ChangeEmailDto
    {
        public string NewEmail { get; set; }
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
    }
}