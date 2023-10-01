using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.DTOs.Item;
using Template.Core.Application.Request;
using Template.Core.Application.Specification;
using Template.Core.Application.Wrappers;
using Template.Core.Domain.Constants;
using Template.Core.Domain.Entities;
using ILogger = Serilog.ILogger;

namespace Template.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ItemsController : ControllerBase
    {
        #region Private Variable

        private readonly IItemServiceAsync _itemServiceAsync;
        private readonly ILogger _logger;

        #endregion Private Variable

        #region Constructor

        public ItemsController(IItemServiceAsync itemServiceAsync, ILogger logger)
        {
            _itemServiceAsync = itemServiceAsync;
            _logger = logger;
        }

        #endregion Constructor

        #region Actions

        // DELETE api/Items/{id}
        [Authorize(Permission.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            await _itemServiceAsync.DeleteAsync(id);
            return NoContent();
        }

        // GET api/Items/{id}
        //[Authorize(Permission.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ItemDetailsDTO>>> GetItemAsync(Guid id)
        {
            var item = await _itemServiceAsync.GetByIdAsync<Guid, ItemDetailsDTO>(id);
            var response = new ApiResponse<ItemDetailsDTO>(item);
            return Ok(response);
        }

        // GET api/Items/
        //[Authorize(Permission.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ItemListDTO>>>> GetAllItemsAsync([FromQuery] ItemRequest request)
        {
            var data = await _itemServiceAsync.GetAllWithPaginationAsync<ItemListDTO, ItemSpecification>(request);
            var response = new ApiResponse<PagedResult<ItemListDTO>>(data);
            return Ok(response);
        }

        // POST api/Items/
        //[Authorize(Permission.Create)]
        [HttpPost]
        public async Task<ActionResult<ItemDetailsDTO>> CreateItemAsync([FromBody] ItemCreateDTO itemCreateDTO)
        {
            var item = await _itemServiceAsync.CreateAsync<ItemCreateDTO, ItemDetailsDTO>(itemCreateDTO);
            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item);
        }

        // PUT api/Items/{id}
        [Authorize(Permission.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] ItemUpdateDTO itemUpdateDTO)
        {
            await _itemServiceAsync.UpdateAsync(id, itemUpdateDTO);
            return NoContent();
        }

        #endregion Actions
    }
}