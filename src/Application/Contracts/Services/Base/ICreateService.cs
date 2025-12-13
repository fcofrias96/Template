namespace Template.Application.Contracts.Services.Base
{
    public interface ICreateService<TEntity>
    {
        Task<TEntity> CreateAsync<TSource>(TSource source, CancellationToken cancellationToken);
    }
}