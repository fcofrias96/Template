namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportToExcelAsync<TEntity, TDbContext>
    {
        Task<byte[]> ExportToExcelAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters) where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToExcelAsync<TDestination>(string title);
    }
}