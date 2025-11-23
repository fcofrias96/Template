using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Services.V1
{
    public class ActionServiceAsync : IActionServiceAsync
    {
        private readonly IActionRepositoryAsync _readRepository;

        public ActionServiceAsync(IActionRepositoryAsync readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<IReadOnlyList<Catalog>> GetProjectedAsync(CancellationToken cancellationToken)
        {
            return await _readRepository.GetProjectedAsync<Catalog>(cancellationToken);
        }
    }
}