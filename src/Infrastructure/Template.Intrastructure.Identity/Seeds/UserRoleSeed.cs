using Microsoft.AspNetCore.Identity;
using Template.Application.Constants;

namespace Template.Identity.Seeds
{
    public static class UserRoleSeed
    {
        public static List<IdentityUserRole<string>> Data { get; } = new List<IdentityUserRole<string>>
        {
            #region Admin

               new IdentityUserRole<string>
               {
                    UserId = UserSeed.Data.First(x => x.UserName =="johnDoe").Id,
                    RoleId = RoleSeed.Data.First(x=> x.Name == Roles.Admin).Id
               },
               new IdentityUserRole<string>
               {
                    UserId = UserSeed.Data.First(x => x.UserName =="johnDoe").Id,
                    RoleId = RoleSeed.Data.First(x=> x.Name == Roles.User).Id
               },

            #endregion Admin

            #region User

            new IdentityUserRole<string>
            {
                UserId = UserSeed.Data.First(x => x.UserName =="johnJames").Id,
                RoleId = RoleSeed.Data.First(x=> x.Name == Roles.User).Id
            }

            #endregion User

        };
    }
}