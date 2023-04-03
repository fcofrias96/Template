using Microsoft.AspNetCore.Identity;
using Template.Infrastructure.Identity.Entity;

namespace Template.Infrastructure.Identity.Seeds
{
    public static class UserSeed
    {
        private static PasswordHasher<User> hasher = new PasswordHasher<User>();

        public static List<User> data = new List<User>
        {
             new User
            {
                FirstName = "John",
                LastName = "Smith",
                UserName = "basicuser",
                NormalizedUserName = "BASICUSER",
                Email = "basicuser@gmail.com",
                NormalizedEmail = "BASICUSER@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new User(),"123Pa$$word")
            },
             new User
            {
                FirstName = "John",
                LastName = "James",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new User(),"123Pa$$word")
            },new User
            {
                FirstName = "John",
                LastName = "Johnson",
                UserName = "moderator",
                NormalizedUserName = "MODERATOR",
                Email = "moderator@gmail.com",
                NormalizedEmail = "MODERATOR@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new User(),"123Pa$$word")
            }
        };
    }
}