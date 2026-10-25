using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Infrastructure.Seeds
{
    /// <summary>
    /// Every value is a constant: seed data is baked into the migrations, so anything generated at runtime
    /// (Guid.NewGuid, password salts) would make the model differ from the snapshot on every build.
    /// </summary>
    public static class UserSeed
    {
        /// <summary>PasswordHasher (v3) hash of "123Pa$$word".</summary>
        private const string DefaultPasswordHash = "AQAAAAIAAYagAAAAEB+04Z9fluXlNeQMiVK4U8DixMzGOAWDDIW8lrAKGnKVV/wLeqd3j3+dQN0eWZ96bA==";

        public static List<ApplicationUser> Data { get; } = new List<ApplicationUser>
        {
            CreateUser(
                id: 1,
                publicId: new Guid("6f0c3d2e-1a4b-4c8d-9e1f-2a3b4c5d6e01"),
                firstName: "John",
                lastName: "Doe",
                userName: "johnDoe",
                email: "johnDoe@gmail.com",
                securityStamp: "5b1f7c9a-3d2e-4f60-8a71-b2c3d4e5f601",
                concurrencyStamp: "9c2e8d1b-4f3a-4b70-9c82-c3d4e5f60701"
            ),
            CreateUser(
                id: 2,
                publicId: new Guid("6f0c3d2e-1a4b-4c8d-9e1f-2a3b4c5d6e02"),
                firstName: "John",
                lastName: "James",
                userName: "johnJames",
                email: "johnJames@gmail.com",
                securityStamp: "5b1f7c9a-3d2e-4f60-8a71-b2c3d4e5f602",
                concurrencyStamp: "9c2e8d1b-4f3a-4b70-9c82-c3d4e5f60702"
            )
        };

        private static ApplicationUser CreateUser(int id, Guid publicId, string firstName, string lastName, string userName, string email, string securityStamp, string concurrencyStamp)
        {
            return new ApplicationUser
            {
                Id = id,
                PublicId = publicId,
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,
                RefreshToken = "",
                SecurityStamp = securityStamp,
                ConcurrencyStamp = concurrencyStamp,
                PasswordHash = DefaultPasswordHash
            };
        }
    }
}
