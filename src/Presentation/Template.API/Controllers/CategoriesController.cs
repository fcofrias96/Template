using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Attributes;
using Template.Api.Controllers.Common;
using Template.Application.Constants;
using Template.Application.Enums;
using Template.Application.Models.DTOs;
using Template.Application.Models.DTOs.V1.Categories;
using Template.Application.Models.Specification;

using Template.Application.Models.Wrappers;
using Template.Domain.Entities.V1;
using Template.Persistence.Contracts.Services;

namespace Template.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoriesController :  ApiControllerBase
    {
        #region Private Variable

        private readonly ICategoryService _categoryServiceAsync;

        #endregion Private Variable

        public CategoriesController(ICategoryService categoryServiceAsync)
        {
            _categoryServiceAsync = categoryServiceAsync;
        }

        #region Actions

        [Permission(Resources.Category, ActionType.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateCategoryAsync([FromBody] CategoryCreateDto categoryCreateDTO, CancellationToken cancellationToken)
        {
            var category = await _categoryServiceAsync.CreateAsync<CategoryCreateDto>(categoryCreateDTO, cancellationToken);
            return CreatedAtAction(nameof(GetCategoryAsync), new { id = category.Id }, category);
        }

        [Permission(Resources.Category, ActionType.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
        {
            await _categoryServiceAsync.SoftDeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Category, ActionType.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryListDto>>>> GetProjectedPaginatedAsync([FromQuery] CategoryQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Category>(request);
            var response = await _categoryServiceAsync.GetProjectedPaginatedAsync<CategoryListDto>(specification, cancellationToken);
            return Ok(response);
		}

        [Permission(Resources.Category, ActionType.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> GetCategoryAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _categoryServiceAsync.GetByIdProjectedAsync<int, CategoryDetailDto>(id, cancellationToken);
            return Ok(response);
        }

        [Permission(Resources.Category, ActionType.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategoryAsync(int id, [FromBody] CategoryUpdateDto categoryUpdateDTO, CancellationToken cancellationToken)
        {
            await _categoryServiceAsync.UpdateAsync(id, categoryUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(Resources.Resource, ActionType.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<Catalog>>>> GetProjectedAsync(CancellationToken cancellationToken)
        {
            var result = await _categoryServiceAsync.GetProjectedWithEtagAsync<Catalog>(cancellationToken);
            return FromEtagResult(result);
        }

        [Permission(Resources.Category, ActionType.Export)]
        [HttpGet("export/docx")]
        public async Task<FileResult> ExportToWordAsync(CancellationToken cancellationToken)
        {
            var file = await _categoryServiceAsync.ExportToDocxAsync<CategoryExportDto>("Lista de Categorias", cancellationToken);
            return File(file, FileContentType.DOC, "Categories.docx");
        }

        [Permission(Resources.Category, ActionType.Export)]
        [HttpGet("export/pdf")]
        public async Task<FileResult> ExportToPdfAsync(CancellationToken cancellationToken)
        {
            var file = await _categoryServiceAsync.ExportToPdfAsync<CategoryExportDto>("Lista de Categorias", cancellationToken);
            return File(file, FileContentType.PDF, "Categories.pdf");
        }

        [Permission(Resources.Category, ActionType.Export)]
        [HttpGet("export/csv")]
        public async Task<FileResult> ExportToCSVAsync(CancellationToken cancellationToken)
        {
            var file = await _categoryServiceAsync.ExportToCSVAsync<CategoryExportDto>("Categorias", cancellationToken);
            return File(file, FileContentType.CSV, "Categories.csv");
        }

        #endregion Actions
    }
}