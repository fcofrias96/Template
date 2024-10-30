namespace Template.Application.Contracts.Services.Base.Export
{
    public interface IExportServiceAsync<TEntity> :
        IExportToExcelAsync<TEntity>,
        IExportToPdfAsync<TEntity>,
        IExportToWordAsync<TEntity>
    { }
}