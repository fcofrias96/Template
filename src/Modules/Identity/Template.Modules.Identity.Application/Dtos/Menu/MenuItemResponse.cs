namespace Template.Modules.Identity.Application.Dtos.Menu
{
    public class MenuItemResponse
    {
        public List<MenuItemDto> Menus { get; set; }

        public bool IsAuthenticated { get; set; }
    }
}
