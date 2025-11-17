using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Actions;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;

namespace Template.Api.Controllers.Security
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize(Roles = Roles.Admin)]
    public class ActionsController : ControllerBase
    {
        private readonly IActionServiceAsync _actionService;

        public ActionsController(IActionServiceAsync actionService)
        {
            _actionService = actionService;
        }

        [Permission(Resources.Action, ActionType.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<List<Catalog>>>> GetActions(CancellationToken cancellationToken)
        {
            var actions = await _actionService.GetProjectedAsync(cancellationToken);
            return Ok(actions);
        }
    }
}