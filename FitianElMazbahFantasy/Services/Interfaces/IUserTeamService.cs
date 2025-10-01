using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Services.Interfaces;

public interface IUserTeamService
{
    Task<UserTeam?> GetUserTeamByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserTeam?> GetUserTeamWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<UserTeam?> GetUserTeamByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserTeam?> GetUserTeamWithDetailsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserTeam> CreateUserTeamAsync(UserTeam userTeam, CancellationToken cancellationToken = default);
    Task<UserTeam> UpdateUserTeamAsync(UserTeam userTeam, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserTeamAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UserHasTeamAsync(int userId, CancellationToken cancellationToken = default);
}