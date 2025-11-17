namespace Template.Application.Contracts.Services.Base
{
    public interface ICreateServiceAsync<TEntity>
    {
        Task<TEntity> CreateAsync<TSource>(TSource source, CancellationToken cancellationToken);
    }
}