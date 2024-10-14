using Template.Application.Extensions;
using Template.Application.Enums;
using Template.Identity.Entities;

namespace Template.Identity.Seeds
{
    public static class PermissionSeed
    {
        public static List<Permission> Data { get; } = new List<Permission>
        {
            #region User

            new Permission
             {
                Id = 1,
                ActionId =  Actions.Create.ToInt() ,
                ResourceId = Resources.User.ToInt()

             },
             new Permission
             {
                 Id = 2,
                 ActionId = Actions.Read.ToInt(),
                 ResourceId = Resources.User.ToInt()
             },
             new Permission
             {
                Id = 3,
                ActionId = Actions.Update.ToInt(),
                ResourceId = Resources.User.ToInt()
             },
             new Permission
             {
                 Id = 4,
                 ActionId = Actions.Delete.ToInt(),
                 ResourceId = Resources.User.ToInt()
             },
             new Permission
             {
                 Id = 5,
                 ActionId = Actions.Export.ToInt(),
                 ResourceId = Resources.User.ToInt()
             },
             #endregion User 

            #region Resource 

             new Permission
             {
                Id = 6,
                ActionId = Actions.Create.ToInt(),
                ResourceId = Resources.Resource.ToInt()

             },
             new Permission
             {
                 Id = 7,
                 ActionId = Actions.Read.ToInt(),
                 ResourceId = Resources.Resource.ToInt()
             },
             new Permission
             {
                Id = 8,
                ActionId = Actions.Update.ToInt(),
                ResourceId = Resources.Resource.ToInt()
             },
             new Permission
             {
                 Id = 9,
                 ActionId = Actions.Delete.ToInt(),
                 ResourceId = Resources.Resource.ToInt()
             },
             new Permission
             {
                 Id = 10,
                 ActionId = Actions.Export.ToInt(),
                 ResourceId = Resources.Resource.ToInt()
             },
             #endregion Resource 


            #region Role

            new Permission
            {
               Id = 11,
               ActionId = Actions.Create.ToInt(),
               ResourceId = Resources.Role.ToInt()

            },
            new Permission
            {
                Id = 12,
                ActionId = Actions.Read.ToInt(),
                ResourceId = Resources.Role.ToInt()
            },
            new Permission
            {
               Id = 13,
               ActionId = Actions.Update.ToInt(),
               ResourceId = Resources.Role.ToInt()
            },
            new Permission
            {
                Id = 14,
                ActionId = Actions.Delete.ToInt(),
                ResourceId = Resources.Role.ToInt()
            },
            new Permission
            {
                Id = 15,
                ActionId = Actions.Export.ToInt(),
                ResourceId = Resources.Role.ToInt()
            },
            #endregion Role 

            #region Permission

            new Permission
            {
               Id = 16,
               ActionId = Actions.Create.ToInt(),
               ResourceId = Resources.Permission.ToInt()

            },
            new Permission
            {
                Id = 17,
                ActionId = Actions.Read.ToInt(),
                ResourceId = Resources.Permission.ToInt()
            },
            new Permission
            {
               Id = 18,
               ActionId = Actions.Update.ToInt(),
               ResourceId = Resources.Permission.ToInt()
            },
            new Permission
            {
                Id = 19,
                ActionId = Actions.Delete.ToInt(),
                ResourceId = Resources.Permission.ToInt()
            },
            new Permission
            {
                Id = 20,
                ActionId = Actions.Export.ToInt(),
                ResourceId = Resources.Permission.ToInt()
            },
            #endregion Permission 

            #region Action
            new Permission
            {
                Id = 21,
                ActionId = Actions.Read.ToInt(),
                ResourceId = Resources.Action.ToInt()
            },
            #endregion Action 
            
            #region Category 

              new Permission
             {
                Id = 22,
                ActionId = Actions.Create.ToInt(),
                ResourceId = Resources.Category.ToInt()

             },
             new Permission
             {
                 Id = 23,
                 ActionId = Actions.Read.ToInt(),
                 ResourceId = Resources.Category.ToInt()
             },
             new Permission
             {
                Id = 24,
                ActionId = Actions.Update.ToInt(),
                ResourceId = Resources.Category.ToInt()
             },
             new Permission
             {
                 Id = 25,
                 ActionId = Actions.Delete.ToInt(),
                 ResourceId = Resources.Category.ToInt()
             },
             new Permission
             {
                 Id = 26,
                 ActionId = Actions.Export.ToInt(),
                 ResourceId = Resources.Category.ToInt()
             },
             #endregion Category 

            #region Item

             new Permission
             {
                Id = 27,
                ActionId = Actions.Create.ToInt() ,
                ResourceId = Resources.Item.ToInt()

             },
             new Permission
             {
                 Id = 28,
                 ActionId = Actions.Read.ToInt(),
                 ResourceId = Resources.Item.ToInt()
             },
             new Permission
             {
                Id = 29,
                ActionId = Actions.Update.ToInt(),
                ResourceId = Resources.Item.ToInt()
             },
             new Permission
             {
                 Id = 30,
                 ActionId = Actions.Delete.ToInt(),
                 ResourceId = Resources.Item.ToInt()
             },
             new Permission
             {
                 Id = 31,
                 ActionId = Actions.Export.ToInt(),
                 ResourceId = Resources.Item.ToInt()
             },
             #endregion Item 

        };
    }
}