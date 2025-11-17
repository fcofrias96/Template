using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs.V1.Items;
using Template.Application.Models.Specification.V1;
using Template.Application.Models.Wrappers;
using Template.Persistence.Contracts.Services;

namespace Template.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ItemsController : ControllerBase
    {
        #region Private Variable

        private readonly IItemService _itemServiceAsync;

        #endregion Private Variable

        public ItemsController(IItemService itemServiceAsync)
        {
            _itemServiceAsync = itemServiceAsync;
        }

        #region Actions

        [Permission(Resources.Item, ActionType.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _itemServiceAsync.PermanentDeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Item, ActionType.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ItemDetailDto>>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _itemServiceAsync.GetByIdProjectedAsync<Guid, ItemDetailDto>(id, cancellationToken);
            return Ok(response);
        }

        [Permission(Resources.Item, ActionType.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedCollection<ItemListDto>>>> GetAllAsync([FromQuery] ItemQueryParameters request, CancellationToken cancellationToken)
        {
            var response = await _itemServiceAsync.GetProjectedPaginatedAsync<ItemListDto, ItemSpecification>(request, cancellationToken);
            return Ok(response);
        }

        [Permission(Resources.Item, ActionType.Create)]
        [HttpPost]
        public async Task<ActionResult<ItemDetailDto>> CreateAsync([FromBody] ItemCreateDto itemCreateDTO, CancellationToken cancellationToken)
        {
            var item = await _itemServiceAsync.CreateAsync<ItemCreateDto>(itemCreateDTO, cancellationToken);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [Permission(Resources.Item, ActionType.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] ItemUpdateDto itemUpdateDTO, CancellationToken cancellationToken)
        {
            await _itemServiceAsync.UpdateAsync(id, itemUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Item, ActionType.Export)]
        [HttpGet("export/docx")]
        public async Task<FileResult> ExportToDocxAsync([FromQuery] ItemQueryParameters request, CancellationToken cancellationToken)
        {
            var file = await _itemServiceAsync.ExportToDocxAsync<ItemExportDto, ItemSpecification>("Lista de Artículos", request, cancellationToken);
            return File(file, FileContentType.DOC, "Items.docx");
        }

        [Permission(Resources.Item, ActionType.Export)]
        [HttpGet("export/pdf")]
        public async Task<FileResult> ExportToPdfAsync([FromQuery] ItemQueryParameters request, CancellationToken cancellationToken)
        {
            var file = await _itemServiceAsync.ExportToPdfAsync<ItemExportDto, ItemSpecification>("Lista de Artículos", request, cancellationToken);
            return File(file, FileContentType.PDF, "Items.pdf");
        }

        [Permission(Resources.Item, ActionType.Export)]
        [HttpGet("export/csv")]
        public async Task<FileResult> ExportToCSVAsync([FromQuery] ItemQueryParameters request, CancellationToken cancellationToken)
        {
            var file = await _itemServiceAsync.ExportToCSVAsync<ItemExportDto, ItemSpecification>("Artículos", request, cancellationToken);
            return File(file, FileContentType.CSV, "Items.csv");
        }

        #endregion Actions
    }
}