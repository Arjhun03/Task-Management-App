using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Services.Interfaces;
using Task_Management_App.DTOs.Category;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // POST: api/category
    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryDto request)
    {
        var result =
            await _categoryService.CreateCategoryAsync(request);

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = result.CategoryId },
            result);
    }

    // GET: api/category
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var result =
            await _categoryService.GetAllCategoriesAsync();

        return Ok(result);
    }

    // GET: api/category/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var result =
            await _categoryService.GetCategoryByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Category not found." });

        return Ok(result);
    }

    // PUT: api/category/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        [FromBody] UpdateCategoryDto request)
    {
        var updated =
            await _categoryService.UpdateCategoryAsync(id, request);

        if (!updated)
            return NotFound(new { message = "Category not found." });

        return NoContent();
    }

    // DELETE: api/category/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var deleted =
            await _categoryService.DeleteCategoryAsync(id);

        if (!deleted)
            return NotFound(new { message = "Category not found." });

        return NoContent();
    }
}