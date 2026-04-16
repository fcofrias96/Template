using Microsoft.AspNetCore.Identity;
using Template.Security.Entities;

namespace Template.Security.Seeds
{
    public static class UserSeed
    {
        private static PasswordHasher<ApplicationUser> Hasher = new PasswordHasher<ApplicationUser>();

        public static List<ApplicationUser> Data { get; } = new List<ApplicationUser>
        {
            CreateUser(
                id: 1,
                firstName: "John",
                lastName: "Doe",
                userName: "johnDoe",
                email: "johnDoe@gmail.com",
                password: "123Pa$$word"
            ),
            CreateUser(
                id: 2,
                firstName: "John",
                lastName: "James",
                userName: "johnJames",
                email: "johnJames@gmail.com",
                password: "123Pa$$word"
            )
        };

        private static ApplicationUser CreateUser(int id, string firstName, string lastName, string userName, string email, string password)
        {
            var user = new ApplicationUser
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,
                RefreshToken = "",
                SecurityStamp = Guid.NewGuid().ToString(),      
                ConcurrencyStamp = Guid.NewGuid().ToString() 
            };

            user.PasswordHash = Hasher.HashPassword(user, password);
            return user;
        }
    }
}