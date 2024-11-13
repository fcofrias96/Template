using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs.V1.Actions;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using ActionType = Template.Security.Entities.ActionType;

namespace Template.Security.Services.V1
{
    public class ActionServiceAsync : IActionServiceAsync
    {
        private readonly IReadRepositoryAsync<ActionType, SecurityDbContext> _readRepository;

        public ActionServiceAsync(IReadRepositoryAsync<ActionType, SecurityDbContext> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<IReadOnlyList<ActionListDto>> GetProjectedAsync()
        {
            return await _readRepository.GetProjectedAsync<ActionListDto>();
        }
    }
}
