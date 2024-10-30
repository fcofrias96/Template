using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs.V1.Actions;
using Template.Identity.Contracts.Services.V1;
using Action = Template.Identity.Entities.Action;

namespace Template.Identity.Services.V1
{
    public class ActionServiceAsync : IActionServiceAsync
    {
        private readonly IReadRepositoryAsync<Action> _readRepository;

        public ActionServiceAsync(IReadRepositoryAsync<Action> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<IReadOnlyList<ActionListDto>> GetProjectedAsync()
        {
            return await _readRepository.GetProjectedAsync<ActionListDto>();
        }
    }
}
       