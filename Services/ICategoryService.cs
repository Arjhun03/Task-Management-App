using Task_Management_App.DTOs.Category;

namespace TaskManagement.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto request);

    Task<List<CategoryResponseDto>> GetAllCategoriesAsync();

    Task<CategoryResponseDto?> GetCategoryByIdAsync(int categoryId);

    Task<bool> UpdateCategoryAsync(int categoryId, UpdateCategoryDto request);

    Task<bool> DeleteCategoryAsync(int categoryId);
}