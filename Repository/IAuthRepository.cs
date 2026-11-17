using TaskManagement.Api.Models;

namespace Task_Management_App.Repository.RepositoryInterface;

public interface IAuthRepository
{
    Task<Users?> GetUserByEmailAsync(string email);

    Task AddUserAsync(Users user);
}