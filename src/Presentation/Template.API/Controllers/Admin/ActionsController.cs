using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs.V1.Actions;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;

namespace Template.Api.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = Roles.Admin)]
    public class ActionsController : ControllerBase
    {

        private readonly IActionServiceAsync _actionService;

        public ActionsController(IActionServiceAsync actionService)
        {
            _actionService = actionService;
        }

        [Permission(Resources.Action, Actions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ActionListDto>>>> GetActions()
        {
            var actions = await _actionService.GetAllProjectedAsync<ActionListDto>();
            return Ok(actions);
        }

    }
}
