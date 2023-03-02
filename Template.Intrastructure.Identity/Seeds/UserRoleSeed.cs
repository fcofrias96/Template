using Microsoft.AspNetCore.Identity;
using Template.Core.Application.Constants;

namespace Template.Infrastructure.Identity.Seeds
{
    public static class UserRoleSeed
    {
        public static List<IdentityUserRole<string>> data = new List<IdentityUserRole<string>>
        {
            #region Super User
            new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="superuser").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.SuperAdmin).Id
            },
            new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="superuser").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Admin).Id
            },
            new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="superuser").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Basic).Id
            },
            new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="superuser").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Moderator).Id
            },
            #endregion Super User
           
            #region Admin
               new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="admin").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Admin).Id
            },
                new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="admin").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Basic).Id
            },
                  new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="admin").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Moderator).Id
            },
            #endregion Admin

            #region Moderator
                              
             new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="moderator").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Moderator).Id
            },
                          new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="moderator").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Basic).Id
            },
            #endregion Moderator

            #region Basic User
            new IdentityUserRole<string>
            {
                UserId = UserSeed.data.First(x => x.UserName =="basicuser").Id,
                RoleId = RoleSeed.data.First(x=> x.Name == Role.Basic).Id
            }
            #endregion Basic User

        };
    }
}