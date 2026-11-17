using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;
using Task_Management_App.DTOs.Category;
using Task_Management_App.Repository;
using TaskManagement.API.Services;

namespace TaskManagement.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
    }

    // CREATE CATEGORY
    public async Task<CategoryResponseDto> CreateCategoryAsync(
        CreateCategoryDto request)
    {
        var category = new Category
        {
            UserId = _currentUserService.UserId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddCategoryAsync(category);

        return MapToDto(category);
    }

    // GET ALL CATEGORIES
    public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var userId = _currentUserService.UserId;

        var categories =
            await _categoryRepository.GetAllCategoriesAsync(userId);

        return categories.Select(MapToDto).ToList();
    }

    // GET CATEGORY BY ID
    public async Task<CategoryResponseDto?> GetCategoryByIdAsync(
        int categoryId)
    {
        var userId = _currentUserService.UserId;

        var category =
            await _categoryRepository.GetCategoryByIdAsync(
                categoryId, userId);

        if (category == null)
            return null;

        return MapToDto(category);
    }

    // UPDATE CATEGORY
    public async Task<bool> UpdateCategoryAsync(
        int categoryId,
        UpdateCategoryDto request)
    {
        var userId = _currentUserService.UserId;

        var category =
            await _categoryRepository.GetCategoryByIdAsync(
                categoryId, userId);

        if (category == null)
            return false;

        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();

        await _categoryRepository.UpdateCategoryAsync(category);

        return true;
    }

    // DELETE CATEGORY
    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        var userId = _currentUserService.UserId;

        var category =
            await _categoryRepository.GetCategoryByIdAsync(
                categoryId, userId);

        if (category == null)
            return false;

        await _categoryRepository.DeleteCategoryAsync(category);

        return true;
    }

    // ENTITY TO DTO MAPPING
    private static CategoryResponseDto MapToDto(Category category)
    {
        return new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt
        };
    }
}