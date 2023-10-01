using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Template.Api.Attributes;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.DTOs.Category;
using Template.Core.Domain.Constants;

namespace Template.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoriesController : ControllerBase
    {
        #region Private Variable

        private readonly IMemoryCache _cache;
        private readonly ICategoryServiceAsync _categoryServiceAsync;

        #endregion Private Variable

        #region Constructor

        public CategoriesController(ICategoryServiceAsync categoryServiceAsync)
        {
            _categoryServiceAsync = categoryServiceAsync;
        }

        #endregion Constructor

        #region Actions

        // POST api/Categories/
        //[Authorize(Permission.Create)]
        [HttpPost]
        public async Task<ActionResult<CategoryDetailsDTO>> CreateCategoryAsync([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
            var category = await _categoryServiceAsync.CreateAsync<CategoryCreateDTO, CategoryDetailsDTO>(categoryCreateDTO);
            return CreatedAtAction(nameof(GetCategoryAsync), new { id = category.Id }, category);
        }

        // DELETE api/Categories/{id}
        //[Authorize(Permission.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(int id)
        {
            await _categoryServiceAsync.DeleteAsync(id);
            return NoContent();
        }

        // GET api/Categories/
        //[Authorize(Permission.Read)]
        [HttpGet]
        [CacheControl]
        public async Task<ActionResult<List<CategoryListDTO>>> GetAllCategoriesAsync()
        {
            var (categoryList, eTag) = await _categoryServiceAsync.GetAllWithETagAsync<CategoryListDTO>();
            Response.Headers.Add("X-Validate-ETag", eTag);
            return Ok(categoryList);
        }

        // GET api/Categories/{id}
        //[Authorize(Permission.Read)]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDetailsDTO>> GetCategoryAsync(int id)
        {
            var category = await _categoryServiceAsync.GetByIdAsync<int, CategoryDetailsDTO>(id);
            return Ok(category);
        }

        // PUT api/Categories/{id}
        [Authorize(Permission.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategoryAsync(int id, [FromBody] CategoryUpdateDTO categoryUpdateDTO)
        {
            await _categoryServiceAsync.UpdateAsync(id, categoryUpdateDTO);
            return NoContent();
        }

        #endregion Actions
    }
}