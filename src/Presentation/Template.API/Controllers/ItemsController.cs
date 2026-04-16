using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Api.Controllers.Common;
using Template.Application.Constants;
using Template.Application.Models.DTOs.V1.Items;
using Template.Application.Models.Specification;

using Template.Application.Models.Wrappers;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Services;
using Template.Security.Entities;

namespace Template.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ItemsController :  ApiControllerBase
    {
        #region Private Variable

        private readonly IItemService _itemServiceAsync;

        #endregion Private Variable

        public ItemsController(IItemService itemServiceAsync)
        {
            _itemServiceAsync = itemServiceAsync;
        }

        #region Actions

        [Permission(Resource.Item, PermissionAction.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _itemServiceAsync.PermanentDeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [Permission(Resource.Item, PermissionAction.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ItemDetailDto>>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _itemServiceAsync.GetByIdProjectedAsync<ItemDetailDto>(id, cancellationToken);
            return Ok(response);
        }

        [Permission(Resource.Item, PermissionAction.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedCollection<ItemListDto>>>> GetAllAsync([FromQuery] ItemQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Item>(request);
            var response = await _itemServiceAsync.GetProjectedPaginatedAsync<ItemListDto>(specification, cancellationToken);
            return Ok(response);
        }

        [Permission(Resource.Item, PermissionAction.Create)]
        [HttpPost]
        public async Task<ActionResult<ItemDetailDto>> CreateAsync([FromBody] ItemCreateDto itemCreateDTO, CancellationToken cancellationToken)
        {
            var item = await _itemServiceAsync.CreateAsync<ItemCreateDto>(itemCreateDTO, cancellationToken);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [Permission(Resource.Item, PermissionAction.Update)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] ItemUpdateDto itemUpdateDTO, CancellationToken cancellationToken)
        {
            await _itemServiceAsync.UpdateAsync(id, itemUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(Resource.Item, PermissionAction.Export)]
        [HttpGet("export/docx")]
        public async Task<FileResult> ExportToDocxAsync([FromQuery] ItemQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Item>(request);
            var file = await _itemServiceAsync.ExportToDocxAsync<ItemExportDto>("Lista de Artículos", specification, cancellationToken);
            return File(file, FileContentType.DOC, "Items.docx");
        }

        [Permission(Resource.Item, PermissionAction.Export)]
        [HttpGet("export/pdf")]
        public async Task<FileResult> ExportToPdfAsync([FromQuery] ItemQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Item>(request);
            var file = await _itemServiceAsync.ExportToPdfAsync<ItemExportDto>("Lista de Artículos", specification, cancellationToken);
            return File(file, FileContentType.PDF, "Items.pdf");
        }

        [Permission(Resource.Item, PermissionAction.Export)]
        [HttpGet("export/csv")]
        public async Task<FileResult> ExportToCSVAsync([FromQuery] ItemQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Item>(request);
            var file = await _itemServiceAsync.ExportToCSVAsync<ItemExportDto>("Artículos", specification, cancellationToken);
            return File(file, FileContentType.CSV, "Items.csv");
        }

        #endregion Actions
    }
}