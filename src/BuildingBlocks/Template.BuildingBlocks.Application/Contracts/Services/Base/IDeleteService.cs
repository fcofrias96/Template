using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Application.Contracts.Services.Base
{
    public interface IDeleteService
    {
        Task<Result> SoftDeleteAsync(int id, CancellationToken cancellationToken);
        Task<Result> PermanentDeleteAsync(int id, CancellationToken cancellationToken);
    }
}
