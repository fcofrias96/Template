using Template.Identity.Entities;

namespace Template.Identity.Seeds
{
    public static class UserModulePermissionSeed
    {
        public static List<UserModulePermission> Data { get; } = new List<UserModulePermission>
        {
            #region Admin

             new UserModulePermission
             {
                UserId = UserSeed.Data.First(x=> x.UserName == "johnDoe").Id,
                ModuleId = ModuleSeed.Data.First(x=> x.Name == "Users").Id,
                PermissionId = PermissionSeed.Data.First(x=> x.Name == "All").Id
             },
             new UserModulePermission
             {
                UserId = UserSeed.Data.First(x=> x.UserName == "johnDoe").Id,
                ModuleId = ModuleSeed.Data.First(x=> x.Name == "Modules").Id,
                PermissionId = PermissionSeed.Data.First(x=> x.Name == "All").Id
             },
             new UserModulePermission
             {
                UserId = UserSeed.Data.First(x=> x.UserName == "johnDoe").Id,
                ModuleId = ModuleSeed.Data.First(x=> x.Name == "Items").Id,
                PermissionId = PermissionSeed.Data.First(x=> x.Name == "All").Id
             },
             new UserModulePermission
             {
                UserId = UserSeed.Data.First(x=> x.UserName == "johnDoe").Id,
                ModuleId = ModuleSeed.Data.First(x=> x.Name == "Categories").Id,
                PermissionId = PermissionSeed.Data.First(x=> x.Name == "All").Id
             },

            #endregion Admin

            #region User

            new UserModulePermission
            {
                UserId = UserSeed.Data.First(x=> x.UserName == "johnJames").Id,
                ModuleId = ModuleSeed.Data.First(x=> x.Name == "Users").Id,
                PermissionId = PermissionSeed.Data.First(x=> x.Name == "Read").Id
            },
            new UserModulePermission
            {
                UserId = UserSeed.Data.First(x=> x.UserName == "johnJames").Id,
                ModuleId = ModuleSeed.Data.First(x=> x.Name == "Items").Id,
                PermissionId = PermissionSeed.Data.First(x=> x.Name == "Read").Id
            },
            new UserModulePermission
            {
                UserId = UserSeed.Data.First(x=> x.UserName == "johnJames").Id,
                ModuleId = ModuleSeed.Data.First(x=> x.Name == "Categories").Id,
                PermissionId = PermissionSeed.Data.First(x=> x.Name == "Read").Id
            }
            #endregion User

        };
    }
}