using Microsoft.AspNetCore.Identity;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Seeds
{
    public static class UserSeed
    {
        private static PasswordHasher<ApplicationUser> Hasher = new PasswordHasher<ApplicationUser>();

        public static List<ApplicationUser> Data { get; } = new List<ApplicationUser>
        {
             new ApplicationUser
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johnDoe",
                NormalizedUserName = "JOHNDOE",
                Email = "johnDoe@gmail.com",
                NormalizedEmail = "JOHNDOE@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = Hasher.HashPassword(new ApplicationUser(),"123Pa$$word"),
                RefreshToken = ""
            },
             new ApplicationUser
            {
                Id = 2,
                FirstName = "John",
                LastName = "James",
                UserName = "johnJames",
                NormalizedUserName = "JOHNJAMES",
                Email = "johnJames@gmail.com",
                NormalizedEmail = "JOHNJAMES@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = Hasher.HashPassword(new ApplicationUser(),"123Pa$$word"),
                RefreshToken = ""
            }
        };
    }
}