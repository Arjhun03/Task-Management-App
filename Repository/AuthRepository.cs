using Microsoft.EntityFrameworkCore;
using Task_Management_App.Repository.RepositoryInterface;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;


namespace TaskManagement.API.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _context;

    public AuthRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddUserAsync(Users user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();
    }
}