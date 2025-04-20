namespace Template.Security.Dtos.Menu
{
    public class MenuItemResponse
    {
        public List<MenuItemDto> Menus { get; set; }

        public bool IsAuthenticated { get; set; }
    }

}