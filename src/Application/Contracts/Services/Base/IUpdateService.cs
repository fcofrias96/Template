namespace Template.Application.Contracts.Services.Base
{
    public interface IUpdateService
    {
        Task UpdateAsync<TSource>(int id, TSource source, CancellationToken cancellationToken);
    }
}