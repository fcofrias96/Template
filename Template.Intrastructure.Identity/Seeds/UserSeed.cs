using Microsoft.AspNetCore.Identity;
using Template.Infrastructure.Identity.Models;

namespace Template.Infrastructure.Identity.Seeds
{
    public static class UserSeed
    {
        private static PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();

        public static List<ApplicationUser> data = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "superuser",
                NormalizedUserName = "SUPERUSER",
                Email = "superuser@gmail.com",
                NormalizedEmail = "SUPERUSER@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new ApplicationUser(),"123Pa$$word")
            },
             new ApplicationUser
            {
                FirstName = "John",
                LastName = "Smith",
                UserName = "basicuser",
                NormalizedUserName = "BASICUSER",
                Email = "basicuser@gmail.com",
                NormalizedEmail = "BASICUSER@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new ApplicationUser(),"123Pa$$word")
            },
             new ApplicationUser
            {
                FirstName = "John",
                LastName = "James",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new ApplicationUser(),"123Pa$$word")
            },new ApplicationUser
            {
                FirstName = "John",
                LastName = "Johnson",
                UserName = "moderator",
                NormalizedUserName = "MODERATOR",
                Email = "moderator@gmail.com",
                NormalizedEmail = "MODERATOR@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new ApplicationUser(),"123Pa$$word")
            }

        };
    }
}