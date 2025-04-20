namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportServiceAsync<TEntity, TDbContext> :
        IExportToCsvAsync<TEntity, TDbContext>,
        IExportToPdfAsync<TEntity, TDbContext>,
        IExportToDocxAsync<TEntity, TDbContext>
    { }
}