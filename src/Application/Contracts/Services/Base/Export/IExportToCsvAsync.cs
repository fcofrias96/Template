namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportToCsvAsync<TEntity, TDbContext>
    {
        Task<byte[]> ExportToCSVAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters) where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToCSVAsync<TDestination>(string title);
    }
}