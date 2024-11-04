namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportToPdfAsync<TEntity, TDbContext>
    {
        Task<byte[]> ExportToPdfAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters) where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToPdfAsync<TDestination>(string title);
    }
}