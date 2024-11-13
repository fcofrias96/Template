using Template.Application.Enums;
using Template.Application.Extensions;
using Template.Security.Entities;

namespace Template.Security.Seeds
{
    public static class ResourceActionSeed
    {
        public static List<ResourceAction> Data { get; } = new List<ResourceAction>
        {
            #region User

            new ResourceAction
             {
                Id = 1,
                ActionId =  Application.Enums.ActionType.Create.ToInt() ,
                ResourceId = Resources.User.ToInt()
             },
             new ResourceAction
             {
                 Id = 2,
                 ActionId = Application.Enums.ActionType.Read.ToInt(),
                 ResourceId = Resources.User.ToInt()
             },
             new ResourceAction
             {
                Id = 3,
                ActionId = Application.Enums.ActionType.Update.ToInt(),
                ResourceId = Resources.User.ToInt()
             },
             new ResourceAction
             {
                 Id = 4,
                 ActionId = Application.Enums.ActionType.Delete.ToInt(),
                 ResourceId = Resources.User.ToInt()
             },
             new ResourceAction
             {
                 Id = 5,
                 ActionId = Application.Enums.ActionType.Export.ToInt(),
                 ResourceId = Resources.User.ToInt()
             },
             #endregion User



            #region Resource

             new ResourceAction
             {
                Id = 6,
                ActionId = Application.Enums.ActionType.Create.ToInt(),
                ResourceId = Resources.Resource.ToInt()
             },
             new ResourceAction
             {
                 Id = 7,
                 ActionId = Application.Enums.ActionType.Read.ToInt(),
                 ResourceId = Resources.Resource.ToInt()
             },
             new ResourceAction
             {
                Id = 8,
                ActionId = Application.Enums.ActionType.Update.ToInt(),
                ResourceId = Resources.Resource.ToInt()
             },
             new ResourceAction
             {
                 Id = 9,
                 ActionId = Application.Enums.ActionType.Delete.ToInt(),
                 ResourceId = Resources.Resource.ToInt()
             },
             new ResourceAction
             {
                 Id = 10,
                 ActionId = Application.Enums.ActionType.Export.ToInt(),
                 ResourceId = Resources.Resource.ToInt()
             },
             #endregion Resource

            #region Role

            new ResourceAction
            {
               Id = 11,
               ActionId = Application.Enums.ActionType.Create.ToInt(),
               ResourceId = Resources.Role.ToInt()
            },
            new ResourceAction
            {
                Id = 12,
                ActionId = Application.Enums.ActionType.Read.ToInt(),
                ResourceId = Resources.Role.ToInt()
            },
            new ResourceAction
            {
               Id = 13,
               ActionId = Application.Enums.ActionType.Update.ToInt(),
               ResourceId = Resources.Role.ToInt()
            },
            new ResourceAction
            {
                Id = 14,
                ActionId = Application.Enums.ActionType.Delete.ToInt(),
                ResourceId = Resources.Role.ToInt()
            },
            new ResourceAction
            {
                Id = 15,
                ActionId = Application.Enums.ActionType.Export.ToInt(),
                ResourceId = Resources.Role.ToInt()
            },
            #endregion Role



            #region ResourceAction

            new ResourceAction
            {
               Id = 16,
               ActionId = Application.Enums.ActionType.Create.ToInt(),
               ResourceId = Resources.ResourceAction.ToInt()
            },
            new ResourceAction
            {
                Id = 17,
                ActionId = Application.Enums.ActionType.Read.ToInt(),
                ResourceId = Resources.ResourceAction.ToInt()
            },
            new ResourceAction
            {
               Id = 18,
               ActionId = Application.Enums.ActionType.Update.ToInt(),
               ResourceId = Resources.ResourceAction.ToInt()
            },
            new ResourceAction
            {
                Id = 19,
                ActionId = Application.Enums.ActionType.Delete.ToInt(),
                ResourceId = Resources.ResourceAction.ToInt()
            },
            new ResourceAction
            {
                Id = 20,
                ActionId = Application.Enums.ActionType.Export.ToInt(),
                ResourceId = Resources.ResourceAction.ToInt()
            },
            #endregion ResourceAction



            #region Action

            new ResourceAction
            {
                Id = 21,
                ActionId = Application.Enums.ActionType.Read.ToInt(),
                ResourceId = Resources.Action.ToInt()
            },
            #endregion Action

            #region Category

              new ResourceAction
             {
                Id = 22,
                ActionId = Application.Enums.ActionType.Create.ToInt(),
                ResourceId = Resources.Category.ToInt()
             },
             new ResourceAction
             {
                 Id = 23,
                 ActionId = Application.Enums.ActionType.Read.ToInt(),
                 ResourceId = Resources.Category.ToInt()
             },
             new ResourceAction
             {
                Id = 24,
                ActionId = Application.Enums.ActionType.Update.ToInt(),
                ResourceId = Resources.Category.ToInt()
             },
             new ResourceAction
             {
                 Id = 25,
                 ActionId = Application.Enums.ActionType.Delete.ToInt(),
                 ResourceId = Resources.Category.ToInt()
             },
             new ResourceAction
             {
                 Id = 26,
                 ActionId = Application.Enums.ActionType.Export.ToInt(),
                 ResourceId = Resources.Category.ToInt()
             },
             #endregion Category



            #region Item

             new ResourceAction
             {
                Id = 27,
                ActionId = Application.Enums.ActionType.Create.ToInt() ,
                ResourceId = Resources.Item.ToInt()
             },
             new ResourceAction
             {
                 Id = 28,
                 ActionId = Application.Enums.ActionType.Read.ToInt(),
                 ResourceId = Resources.Item.ToInt()
             },
             new ResourceAction
             {
                Id = 29,
                ActionId = Application.Enums.ActionType.Update.ToInt(),
                ResourceId = Resources.Item.ToInt()
             },
             new ResourceAction
             {
                 Id = 30,
                 ActionId = Application.Enums.ActionType.Delete.ToInt(),
                 ResourceId = Resources.Item.ToInt()
             },
             new ResourceAction
             {
                 Id = 31,
                 ActionId = Application.Enums.ActionType.Export.ToInt(),
                 ResourceId = Resources.Item.ToInt()
             },
             #endregion Item


            #region Configuration

            new ResourceAction
            {
                Id = 32,
                ActionId = Application.Enums.ActionType.Read.ToInt(),
                ResourceId = Resources.Configuration.ToInt()
            },
            #endregion Configuration
        };
    }
}