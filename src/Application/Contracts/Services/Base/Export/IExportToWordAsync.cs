namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportToWordAsync<TEntity>
    {
        Task<byte[]> ExportToWordAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters) where TSpecification : ISpecification<TEntity>;

        Task<byte[]> ExportToWordAsync<TDestination>(string title);
    }
}