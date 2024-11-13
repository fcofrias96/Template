using Microsoft.AspNetCore.Identity;
using Template.Security.Entities;

namespace Template.Security.Seeds
{
    public static class UserSeed
    {
        private static PasswordHasher<User> Hasher = new PasswordHasher<User>();

        public static List<User> Data { get; } = new List<User>
        {
             new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johnDoe",
                NormalizedUserName = "JOHNDOE",
                Email = "johnDoe@gmail.com",
                NormalizedEmail = "JOHNDOE@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = Hasher.HashPassword(new User(),"123Pa$$word"),
                RefreshToken = "",
                IsDeleted = false
            },
             new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "John",
                LastName = "James",
                UserName = "johnJames",
                NormalizedUserName = "JOHNJAMES",
                Email = "johnJames@gmail.com",
                NormalizedEmail = "JOHNJAMES@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = Hasher.HashPassword(new User(),"123Pa$$word"),
                RefreshToken = "",
                IsDeleted = false
            }
        };
    }
}