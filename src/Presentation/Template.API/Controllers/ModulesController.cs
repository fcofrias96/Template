using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Constants;
using Template.Application.Models.DTOs.V1.Module;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Services.V1;

namespace Template.Api.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ModulesController : ControllerBase
    {
        #region Private Variable

        private readonly IModuleService _ModuleServiceAsync;

        #endregion Private Variable

        public ModulesController(IModuleService ModuleServiceAsync)
        {
            _ModuleServiceAsync = ModuleServiceAsync;
        }

        #region Actions

        [HttpPost]
        public async Task<ActionResult> CreateModuleAsync([FromBody] ModuleCreateDTO ModuleCreateDTO)
        {
            var Module = await _ModuleServiceAsync.CreateAsync(ModuleCreateDTO);
            return CreatedAtAction(nameof(GetModuleAsync), new { id = Module.Id }, Module);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteModuleAsync(int id)
        {
            await _ModuleServiceAsync.DeletePermanentAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ModuleListDTO>>>> GetAllModulesAsync()
        {
            var response = await _ModuleServiceAsync.GetAllProjectedAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ModuleDetailDTO>>> GetModuleAsync(int id)
        {
            var response = await _ModuleServiceAsync.GetByIdProjectedAsync(id);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateModuleAsync(int id, [FromBody] ModuleUpdateDTO moduleUpdateDTO)
        {
            await _ModuleServiceAsync.UpdateAsync(id, moduleUpdateDTO);
            return NoContent();
        }

        #endregion Actions
    }
}