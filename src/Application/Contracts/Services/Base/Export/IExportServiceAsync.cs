namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportServiceAsync<TEntity, TDbContext> :
        IExportToExcelAsync<TEntity, TDbContext>,
        IExportToPdfAsync<TEntity, TDbContext>,
        IExportToWordAsync<TEntity, TDbContext>
    { }
}