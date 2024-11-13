using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Contracts.Services.V1;
using Template.Application.Enums;
using Template.Application.Models.DTOs.V1.Items;
using Template.Application.Models.Specification.V1;
using Template.Application.Models.Wrappers;

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
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _itemServiceAsync.PermanentDeleteAsync(id);
            return NoContent();
        }

        [Permission(Resources.Item, ActionType.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ItemDetailDto>>> GetByIdAsync(Guid id)
        {
            var response = await _itemServiceAsync.GetByIdProjectedAsync<Guid, ItemDetailDto>(id);
            return Ok(response);
        }

        [Permission(Resources.Item, ActionType.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedCollection<ItemListDto>>>> GetAllAsync([FromQuery] ItemQueryParameters request)
        {
            var response = await _itemServiceAsync.GetProjectedPaginatedAsync<ItemListDto, ItemSpecification>(request);
            return Ok(response);
        }

        [Permission(Resources.Item, ActionType.Create)]
        [HttpPost]
        public async Task<ActionResult<ItemDetailDto>> CreateAsync([FromBody] ItemCreateDto itemCreateDTO)
        {
            var item = await _itemServiceAsync.CreateAsync<ItemCreateDto>(itemCreateDTO);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [Permission(Resources.Item, ActionType.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] ItemUpdateDto itemUpdateDTO)
        {
            await _itemServiceAsync.UpdateAsync(id, itemUpdateDTO);
            return NoContent();
        }

        [Permission(Resources.Item, ActionType.Export)]
        [HttpGet("export/word")]
        public async Task<FileResult> ExportToWordAsync([FromQuery] ItemQueryParameters request)
        {
            var file = await _itemServiceAsync.ExportToWordAsync<ItemExportDto, ItemSpecification>("Lista de Artículos", request);
            return File(file, FileContentType.Word, "Items.docx");
        }

        [Permission(Resources.Item, ActionType.Export)]
        [HttpGet("export/pdf")]
        public async Task<FileResult> ExportToPdfAsync([FromQuery] ItemQueryParameters request)
        {
            var file = await _itemServiceAsync.ExportToPdfAsync<ItemExportDto, ItemSpecification>("Lista de Artículos", request);
            return File(file, FileContentType.Pdf, "Items.pdf");
        }

        [Permission(Resources.Item, ActionType.Export)]
        [HttpGet("export/excel")]
        public async Task<FileResult> ExportToExcelAsync([FromQuery] ItemQueryParameters request)
        {
            var file = await _itemServiceAsync.ExportToExcelAsync<ItemExportDto, ItemSpecification>("Artículos", request);
            return File(file, FileContentType.Excel, "Items.xlsx");
        }

        #endregion Actions
    }
}