using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;

namespace Task_Management_App.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // CREATE
    public async Task AddCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    // GET ALL
    public async Task<List<Category>> GetAllCategoriesAsync(int userId)
    {
        return await _context.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    // GET BY ID
    public async Task<Category?> GetCategoryByIdAsync(
        int categoryId, int userId)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.CategoryId == categoryId &&
                c.UserId == userId);
    }

    // UPDATE
    public async Task UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    // DELETE
    public async Task DeleteCategoryAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}