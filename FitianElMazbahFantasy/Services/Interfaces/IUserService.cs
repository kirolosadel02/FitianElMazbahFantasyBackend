using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
}