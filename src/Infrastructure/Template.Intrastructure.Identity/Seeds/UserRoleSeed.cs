using Template.Application.Constants;
using Template.Identity.Entities;

namespace Template.Identity.Seeds
{
    public static class UserRoleSeed
    {
        public static List<UserRole> Data { get; } = new List<UserRole>
        {
            #region Admin

               new UserRole
               {
                    UserId = UserSeed.Data.First(x => x.UserName =="johnDoe").Id,
                    RoleId = RoleSeed.Data.First(x=> x.Name == Roles.Admin).Id
               },
               new UserRole
               {
                    UserId = UserSeed.Data.First(x => x.UserName =="johnDoe").Id,
                    RoleId = RoleSeed.Data.First(x=> x.Name == Roles.User).Id
               },

            #endregion Admin

            #region User

            new UserRole
            {
                UserId = UserSeed.Data.First(x => x.UserName =="johnJames").Id,
                RoleId = RoleSeed.Data.First(x=> x.Name == Roles.User).Id
            }

            #endregion User

        };
    }
}