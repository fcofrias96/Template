using Microsoft.AspNetCore.Identity;
using System.Collections.Immutable;
using Template.Core.Domain.Constants;

namespace Template.Infrastructure.Identity.Seeds
{
    public static class RoleClaimSeed
    {
        public readonly static List<IdentityRoleClaim<string>> data = new List<IdentityRoleClaim<string>>
        {
            #region Admin

            new IdentityRoleClaim<string>
            {   Id = -1,
                ClaimType = "Permission",
                ClaimValue= Permission.Create,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Admin).Id
            },
            new IdentityRoleClaim<string>
            {
                Id = -2,
                ClaimType = "Permission",
                ClaimValue= Permission.Read,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Admin).Id
            },
            new IdentityRoleClaim<string>
            {
                Id = -3,
                ClaimType = "Permission",
                ClaimValue= Permission.Update,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Admin).Id
            },
            new IdentityRoleClaim<string>
            {
                Id = -4,
                ClaimType = "Permission",
                ClaimValue= Permission.Delete,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Admin).Id
            },

            #endregion Admin

            #region Moderator

             new IdentityRoleClaim<string>
            {
                 Id = -5,
                ClaimType = "Permission",
                ClaimValue= Permission.Read,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Moderator).Id
            },
            new IdentityRoleClaim<string>
            {
                Id = -6,
                ClaimType = "Permission",
                ClaimValue= Permission.Update,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Moderator).Id
            },

            #endregion Moderator

            #region Basic User

            new IdentityRoleClaim<string>
            {
                Id = -7,
                ClaimType = "Permission",
                ClaimValue= Permission.Read,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Basic).Id
            }

            #endregion Basic User
        };
    }
}