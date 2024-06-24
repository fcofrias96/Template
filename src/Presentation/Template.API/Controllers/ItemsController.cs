using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Constants;
using Template.Application.Contracts.Services.V1;
using Template.Application.Models.DTOs.V1.Category;
using Template.Application.Models.DTOs.V1.Item;
using Template.Application.Models.DTOs.V1.Item;
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

        [Authorize(Permissions.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _itemServiceAsync.DeletePermanentAsync(id);
            return NoContent();
        }

        [Authorize(Permissions.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ItemDetailDTO>>> GetByIdAsync(Guid id)
        {
            var response = await _itemServiceAsync.GetByIdProjectedAsync<Guid, ItemDetailDTO>(id);
            return Ok(response);
        }

        // GET api/Items/
        [Authorize(Permissions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedCollection<ItemListDTO>>>> GetAllAsync([FromQuery] ItemQueryParameters request)
        {
            var response = await _itemServiceAsync.GetAllProjectedWithPaginationAsync<ItemListDTO, ItemSpecification>(request);
            return Ok(response);
        }

        [Authorize(Permissions.Create)]
        [HttpPost]
        public async Task<ActionResult<ItemDetailDTO>> CreateAsync([FromBody] ItemCreateDTO itemCreateDTO)
        {
            var item = await _itemServiceAsync.CreateAsync<ItemCreateDTO, ItemDetailDTO>(itemCreateDTO);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        // PUT api/Items/{id}
        [Authorize(Permissions.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] ItemUpdateDTO itemUpdateDTO)
        {
            await _itemServiceAsync.UpdateAsync(id, itemUpdateDTO);
            return NoContent();
        }


        [Authorize(Permissions.Export)]
        [HttpGet("export/word")]
        public async Task<FileResult> ExportToWordAsync([FromQuery] ItemQueryParameters request)
        {
            var file = await _itemServiceAsync.ExportToWordAsync<ItemExportDTO, ItemSpecification>("Lista de Artículos", request);
            return File(file, FileContentType.Word, "Items.docx");
        }


        [Authorize(Permissions.Export)]
        [HttpGet("export/pdf")]
        public async Task<FileResult> ExportToPdfAsync([FromQuery] ItemQueryParameters request)
        {
            var file = await _itemServiceAsync.ExportToPdfAsync<ItemExportDTO, ItemSpecification> ("Lista de Artículos", request);
            return File(file, FileContentType.Pdf, "Items.pdf");
        }

        [Authorize(Permissions.Export)]
        [HttpGet("export/excel")]
        public async Task<FileResult> ExportToExcelAsync([FromQuery]ItemQueryParameters request)
        {
            var file = await _itemServiceAsync.ExportToExcelAsync<ItemExportDTO, ItemSpecification>("Artículos", request);
            return File(file, FileContentType.Excel, "Items.xlsx");
        }

        #endregion Actions
    }
}