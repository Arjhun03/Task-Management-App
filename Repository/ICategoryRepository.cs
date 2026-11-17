using TaskManagement.Api.Models;

namespace Task_Management_App.Repository;

public interface ICategoryRepository
{
    Task AddCategoryAsync(Category category);

    Task<List<Category>> GetAllCategoriesAsync(int userId);

    Task<Category?> GetCategoryByIdAsync(int categoryId, int userId);

    Task UpdateCategoryAsync(Category category);

    Task DeleteCategoryAsync(Category category);
}