using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Contracts.Authorization;
using Template.BuildingBlocks.Web.Extensions;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Identity.Application.Dtos.Lookups;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ActionsController : ControllerBase
    {
        private readonly IActionService _actionService;

        public ActionsController(IActionService actionService)
        {
            _actionService = actionService;
        }

        [Permission(PermissionResources.Action, PermissionActions.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetActions(CancellationToken cancellationToken)
        {
            var result = await _actionService.GetProjectedWithEtagAsync<ActionLookupDto>(cancellationToken);
            return result.ToActionResult();
        }
    }
}
