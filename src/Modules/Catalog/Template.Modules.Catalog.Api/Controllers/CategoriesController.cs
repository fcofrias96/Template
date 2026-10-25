using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Modules.Identity.Contracts.Authorization;
using Template.BuildingBlocks.Application.Constants;
using Template.BuildingBlocks.Web.Extensions;
using Template.BuildingBlocks.Application.Models.DTOs;
using Template.Modules.Catalog.Application.Dtos.Categories;
using Template.BuildingBlocks.Application.Models.Specification;

using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Catalog.Domain.Entities;
using Template.Modules.Catalog.Application.Contracts.Services;

namespace Template.Modules.Catalog.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryServiceAsync;
        public CategoriesController(ICategoryService categoryServiceAsync)
        {
            _categoryServiceAsync = categoryServiceAsync;
        }

        #region Actions

        [Permission(PermissionResources.Category, PermissionActions.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateCategoryAsync([FromBody] CategoryCreateDto categoryCreateDTO, CancellationToken cancellationToken)
        {
            var category = await _categoryServiceAsync.CreateAsync<CategoryCreateDto>(categoryCreateDTO, cancellationToken);
            return CreatedAtAction(nameof(GetCategoryAsync), new { id = category.Id }, category);
        }

        [Permission(PermissionResources.Category, PermissionActions.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
        {
            await _categoryServiceAsync.SoftDeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [Permission(PermissionResources.Category, PermissionActions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryListDto>>>> GetProjectedPaginatedAsync([FromQuery] CategoryQueryParameters request, CancellationToken cancellationToken)
        {
            var specification = new Specification<Category>(request);
            var response = await _categoryServiceAsync.GetProjectedPaginatedAsync<CategoryListDto>(specification, cancellationToken);
            return Ok(response);
		}

        [Permission(PermissionResources.Category, PermissionActions.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> GetCategoryAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _categoryServiceAsync.GetByIdProjectedAsync<CategoryDetailDto>(id, cancellationToken);
            return Ok(response);
        }

        [Permission(PermissionResources.Category, PermissionActions.Update)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchCategoryAsync(int id, [FromBody] CategoryPatchDto categoryPatchDTO, CancellationToken cancellationToken)
        {
            await _categoryServiceAsync.PatchAsync(id, categoryPatchDTO, cancellationToken);
            return NoContent();
        }

        [Permission(PermissionResources.Category, PermissionActions.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategoryAsync(int id, [FromBody] CategoryUpdateDto categoryUpdateDTO, CancellationToken cancellationToken)
        {
            await _categoryServiceAsync.UpdateAsync(id, categoryUpdateDTO, cancellationToken);
            return NoContent();
        }

        [Permission(PermissionResources.ResourceItem, PermissionActions.Read)]
        [HttpGet("catalog")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetProjectedAsync(CancellationToken cancellationToken)
        {
            var result = await _categoryServiceAsync.GetProjectedWithEtagAsync<CategoryLookupDto>(cancellationToken);
            return result.ToActionResult();
        }

        [Permission(PermissionResources.Category, PermissionActions.Export)]
        [HttpGet("export/docx")]
        public async Task<FileResult> ExportToWordAsync(CancellationToken cancellationToken)
        {
            var file = await _categoryServiceAsync.ExportToDocxAsync<CategoryExportDto>("Lista de Categorias", cancellationToken);
            return File(file, FileContentType.DOC, "Categories.docx");
        }

        [Permission(PermissionResources.Category, PermissionActions.Export)]
        [HttpGet("export/pdf")]
        public async Task<FileResult> ExportToPdfAsync(CancellationToken cancellationToken)
        {
            var file = await _categoryServiceAsync.ExportToPdfAsync<CategoryExportDto>("Lista de Categorias", cancellationToken);
            return File(file, FileContentType.PDF, "Categories.pdf");
        }

        [Permission(PermissionResources.Category, PermissionActions.Export)]
        [HttpGet("export/csv")]
        public async Task<FileResult> ExportToCSVAsync(CancellationToken cancellationToken)
        {
            var file = await _categoryServiceAsync.ExportToCSVAsync<CategoryExportDto>("Categorias", cancellationToken);
            return File(file, FileContentType.CSV, "Categories.csv");
        }

        #endregion Actions
    }
}
