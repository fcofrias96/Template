using Template.Application.Constants;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Seeds
{
    public static class UserRoleSeed
    {
        public static List<ApplicationUserRole> Data { get; } = new List<ApplicationUserRole>
        {
            #region Admin

               new ApplicationUserRole
               {
                    UserId = UserSeed.Data.First(x => x.UserName =="johnDoe").Id,
                    RoleId = RoleSeed.Data.First(x=> x.Name == Roles.Admin).Id
               },
               new ApplicationUserRole
               {
                    UserId = UserSeed.Data.First(x => x.UserName =="johnDoe").Id,
                    RoleId = RoleSeed.Data.First(x=> x.Name == Roles.User).Id
               },

            #endregion Admin

            #region User

            new ApplicationUserRole
            {
                UserId = UserSeed.Data.First(x => x.UserName =="johnJames").Id,
                RoleId = RoleSeed.Data.First(x=> x.Name == Roles.User).Id
            }

            #endregion User
        };
    }
}