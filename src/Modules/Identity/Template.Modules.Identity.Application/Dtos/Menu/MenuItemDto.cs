namespace Template.Modules.Identity.Application.Dtos.Menu
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int Order { get; set; }
        public List<MenuItemDto> Children { get; set; }
    }
}
