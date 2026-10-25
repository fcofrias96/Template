namespace Template.BuildingBlocks.Application.Models.DTOs
{
    /// <summary>
    /// Id/Name pair used by the "catalog" endpoints that feed dropdowns.
    /// Each module derives an entity-specific type that supplies the projection.
    /// </summary>
    public class LookupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
