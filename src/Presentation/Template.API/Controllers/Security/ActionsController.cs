using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Api.Controllers.Common;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;

namespace Template.Api.Controllers.Security
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ActionsController :  ApiControllerBase
    {
        private readonly IActionServiceAsync _actionService;

        public ActionsController(IActionServiceAsync actionService)
        {
            _actionService = actionService;
        }

        [Permission(Resources.Action, ActionType.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<Catalog>>>> GetActions(CancellationToken cancellationToken)
        {
            var result = await _actionService.GetProjectedWithEtagAsync<Catalog>(cancellationToken);
            return FromEtagResult(result);
        }
    }
}