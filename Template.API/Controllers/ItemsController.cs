using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.Api.Attributes;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.DTOs.Item;
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
        #region Private Varible
        private readonly IItemServiceAsync _itemServiceAsync;
        private readonly ILogger _logger;
        #endregion Private Varible

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
        [Authorize(Permission.Read)]
        [HttpGet("{id}")]
        [CacheControl]
        public async Task<ActionResult<ItemListDTO>> GetItemAsync(Guid id)
        {
            var includeList = new List<string> { "Category" };
            var (item, eTag) = await _itemServiceAsync.GetByIdWithETagAsync<Guid, ItemDetailsDTO>(id, includeList);
            Response.Headers.Add("X-Validate-ETag", eTag);
            return Ok(item);
        }

        // GET api/Items/
        //[Authorize(Permission.Read)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemListDTO>>> GetAllItemsAsync([FromQuery] QueryParameters<ItemListDTO> queryParameters)
        {
            var x = queryParameters.SortableProperties;
            var includeList = new List<string> { "Category" };
            var pagedItemsResult = await _itemServiceAsync.GetAllWithPaginationAsync<ItemListDTO>(queryParameters, includeList);
            return Ok(pagedItemsResult);
        }

        // POST api/Items/
        [Authorize(Permission.Create)]
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