using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Template.Api.Attributes;
using Template.Application.Constants;
using Template.Application.Contracts.Services.V1;
using Template.Application.Enums;
using Template.Application.Models.DTOs.V1.Categories;
using Template.Application.Models.Specification.V1;
using Template.Application.Models.Wrappers;

namespace Template.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoriesController : ControllerBase
    {
        #region Private Variable

        private readonly IMemoryCache _cache;
        private readonly ICategoryService _categoryServiceAsync;

        #endregion Private Variable

        public CategoriesController(ICategoryService categoryServiceAsync)
        {
            _categoryServiceAsync = categoryServiceAsync;
        }

        #region Actions

        [Permission(Resources.Category, Actions.Create)]
        [HttpPost]
        public async Task<ActionResult> CreateCategoryAsync([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
            var category = await _categoryServiceAsync.CreateAsync<CategoryCreateDTO>(categoryCreateDTO);
            return CreatedAtAction(nameof(GetCategoryAsync), new { id = category.Id }, category);
        }

        [Permission(Resources.Category, Actions.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(int id)
        {
            await _categoryServiceAsync.SoftDeleteAsync(id);
            return NoContent();
        }

        [Permission(Resources.Category, Actions.Read)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryListDTO>>>> GetAllCategoriesAsync([FromQuery] CategoryQueryParameters request)
        {
            var response = await _categoryServiceAsync.GetProjectedPaginatedAsync<CategoryListDTO, CategorySpecification>(request);
            return Ok(response);
        }

        [Permission(Resources.Category, Actions.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDetailDTO>>> GetCategoryAsync(int id)
        {
            var response = await _categoryServiceAsync.GetByIdProjectedAsync<int, CategoryDetailDTO>(id);
            return Ok(response);
        }

        [Permission(Resources.Category, Actions.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategoryAsync(int id, [FromBody] CategoryUpdateDTO categoryUpdateDTO)
        {
            await _categoryServiceAsync.UpdateAsync(id, categoryUpdateDTO);
            return NoContent();
        }

        [Permission(Resources.Category, Actions.Export)]
        [HttpGet("export/word")]
        public async Task<FileResult> ExportToWordAsync()
        {
            var file = await _categoryServiceAsync.ExportToWordAsync<CategoryExportDTO>("Lista de Categorias");
            return File(file, FileContentType.Word, "Categories.docx");
        }

        [Permission(Resources.Category, Actions.Export)]
        [HttpGet("export/pdf")]
        public async Task<FileResult> ExportToPdfAsync()
        {
            var file = await _categoryServiceAsync.ExportToPdfAsync<CategoryExportDTO>("Lista de Categorias");
            return File(file, FileContentType.Pdf, "Categories.pdf");
        }

        [Permission(Resources.Category, Actions.Export)]
        [HttpGet("export/excel")]
        public async Task<FileResult> ExportToExcelAsync()
        {
            var file = await _categoryServiceAsync.ExportToExcelAsync<CategoryExportDTO>("Categorias");
            return File(file, FileContentType.Excel, "Categories.xlsx");
        }

        #endregion Actions
    }
}