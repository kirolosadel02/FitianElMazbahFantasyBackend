using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.Services.Interfaces;

namespace FitianElMazbahFantasy.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<User>().GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            throw;
        }
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<User>().FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username {Username}", username);
            throw;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<User>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            throw;
        }
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            // Single query to check existing username or email
            var existingUser = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.UserName == user.UserName || u.Email == user.Email, cancellationToken);

            if (existingUser != null)
            {
                if (existingUser.UserName == user.UserName)
                {
                    throw new InvalidOperationException($"Username '{user.UserName}' already exists");
                }
                if (existingUser.Email == user.Email)
                {
                    throw new InvalidOperationException($"Email '{user.Email}' already exists");
                }
            }

            user.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created successfully with id {UserId}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with username {Username}", user.UserName);
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User updated successfully with id {UserId}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with id {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return false;
            }

            _unitOfWork.Repository<User>().Remove(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User deleted successfully with id {UserId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with id {UserId}", id);
            throw;
        }
    }

    public async Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<User>().AnyAsync(u => u.UserName == username, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if username exists {Username}", username);
            throw;
        }
    }

    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<User>().AnyAsync(u => u.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if email exists {Email}", email);
            throw;
        }
    }
}