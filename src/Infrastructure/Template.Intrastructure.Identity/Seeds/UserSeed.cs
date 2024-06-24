using Microsoft.AspNetCore.Identity;
using Template.Identity.Entities;

namespace Template.Identity.Seeds
{
    public static class UserSeed
    {
        private static PasswordHasher<User> Hasher = new PasswordHasher<User>();

        public static List<User> Data { get; } = new List<User>
        {
             new User
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "johnDoe",
                NormalizedUserName = "JOHNDOE",
                Email = "johnDoe@gmail.com",
                NormalizedEmail = "JOHNDOE@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = Hasher.HashPassword(new User(),"123Pa$$word"),
                Active = true
            },
             new User
            {
                FirstName = "John",
                LastName = "James",
                UserName = "johnJames",
                NormalizedUserName = "JOHNJAMES",
                Email = "johnJames@gmail.com",
                NormalizedEmail = "JOHNJAMES.COM",
                EmailConfirmed = true,
                PasswordHash = Hasher.HashPassword(new User(),"123Pa$$word"),
                Active = true
            }
        };
    }
}