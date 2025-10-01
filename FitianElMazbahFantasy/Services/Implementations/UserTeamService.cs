using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.Services.Interfaces;

namespace FitianElMazbahFantasy.Services.Implementations;

public class UserTeamService : IUserTeamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserTeamService> _logger;

    public UserTeamService(IUnitOfWork unitOfWork, ILogger<UserTeamService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserTeam?> GetUserTeamByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserTeam>().GetByIdAsync(id, cancellationToken, ut => ut.User);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user team by id {UserTeamId}", id);
            throw;
        }
    }

    public async Task<UserTeam?> GetUserTeamWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserTeam>().GetByIdAsync(id, cancellationToken, 
                ut => ut.User, 
                ut => ut.UserTeamPlayers.Select(utp => utp.Player).Select(p => p.Team));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user team with details by id {UserTeamId}", id);
            throw;
        }
    }

    public async Task<UserTeam?> GetUserTeamByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserTeam>().FirstOrDefaultAsync(
                ut => ut.UserId == userId, 
                cancellationToken, 
                ut => ut.User);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user team by user id {UserId}", userId);
            throw;
        }
    }

    public async Task<UserTeam?> GetUserTeamWithDetailsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserTeam>().FirstOrDefaultAsync(
                ut => ut.UserId == userId, 
                cancellationToken, 
                ut => ut.User, 
                ut => ut.UserTeamPlayers.Select(utp => utp.Player).Select(p => p.Team));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user team with details by user id {UserId}", userId);
            throw;
        }
    }

    public async Task<UserTeam> CreateUserTeamAsync(UserTeam userTeam, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user already has a team - this constraint is now enforced at database level too
            var existingTeam = await UserHasTeamAsync(userTeam.UserId, cancellationToken);
            if (existingTeam)
            {
                throw new InvalidOperationException($"User with ID {userTeam.UserId} already has a team. Each user can have only one team.");
            }

            userTeam.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<UserTeam>().AddAsync(userTeam, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return the created team with user info
            var createdTeam = await GetUserTeamByIdAsync(userTeam.Id, cancellationToken);

            _logger.LogInformation("User team created successfully with id {UserTeamId} for user {UserId}", userTeam.Id, userTeam.UserId);
            return createdTeam!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user team for user {UserId}", userTeam.UserId);
            throw;
        }
    }

    public async Task<UserTeam> UpdateUserTeamAsync(UserTeam userTeam, CancellationToken cancellationToken = default)
    {
        try
        {
            userTeam.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<UserTeam>().Update(userTeam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return the updated team with user info
            var updatedTeam = await GetUserTeamByIdAsync(userTeam.Id, cancellationToken);

            _logger.LogInformation("User team updated successfully with id {UserTeamId}", userTeam.Id);
            return updatedTeam!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user team with id {UserTeamId}", userTeam.Id);
            throw;
        }
    }

    public async Task<bool> DeleteUserTeamAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userTeam = await _unitOfWork.Repository<UserTeam>().GetByIdAsync(id, cancellationToken);
            if (userTeam == null)
            {
                return false;
            }

            _unitOfWork.Repository<UserTeam>().Remove(userTeam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User team deleted successfully with id {UserTeamId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user team with id {UserTeamId}", id);
            throw;
        }
    }

    public async Task<bool> UserHasTeamAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserTeam>().AnyAsync(ut => ut.UserId == userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user has team for user {UserId}", userId);
            throw;
        }
    }

    #region Player Management Methods

    public async Task<bool> AddPlayerToTeamAsync(int teamId, int playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if player is already in the team
            var existingPlayerTeam = await _unitOfWork.Repository<UserTeamPlayer>()
                .FirstOrDefaultAsync(utp => utp.UserTeamId == teamId && utp.PlayerId == playerId, cancellationToken);
            
            if (existingPlayerTeam != null)
            {
                return false; // Player already in team
            }

            var userTeamPlayer = new UserTeamPlayer
            {
                UserTeamId = teamId,
                PlayerId = playerId,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<UserTeamPlayer>().AddAsync(userTeamPlayer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Player {PlayerId} added to team {TeamId}", playerId, teamId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding player {PlayerId} to team {TeamId}", playerId, teamId);
            throw;
        }
    }

    public async Task<bool> RemovePlayerFromTeamAsync(int teamId, int playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userTeamPlayer = await _unitOfWork.Repository<UserTeamPlayer>()
                .FirstOrDefaultAsync(utp => utp.UserTeamId == teamId && utp.PlayerId == playerId, cancellationToken);

            if (userTeamPlayer == null)
            {
                return false; // Player not in team
            }

            _unitOfWork.Repository<UserTeamPlayer>().Remove(userTeamPlayer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Player {PlayerId} removed from team {TeamId}", playerId, teamId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing player {PlayerId} from team {TeamId}", playerId, teamId);
            throw;
        }
    }

    public async Task<bool> IsPlayerInTeamAsync(int teamId, int playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserTeamPlayer>()
                .AnyAsync(utp => utp.UserTeamId == teamId && utp.PlayerId == playerId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if player {PlayerId} is in team {TeamId}", playerId, teamId);
            throw;
        }
    }

    public async Task<int> GetTeamPlayerCountAsync(int teamId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserTeamPlayer>()
                .CountAsync(utp => utp.UserTeamId == teamId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player count for team {TeamId}", teamId);
            throw;
        }
    }

    public async Task<int> GetGoalkeeperCountAsync(int teamId, CancellationToken cancellationToken = default)
    {
        try
        {
            var goalkeepers = await _unitOfWork.Repository<UserTeamPlayer>()
                .FindAsync(utp => utp.UserTeamId == teamId, cancellationToken, utp => utp.Player);

            return goalkeepers.Count(utp => utp.Player?.Position == PlayerPosition.Goalkeeper);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting goalkeeper count for team {TeamId}", teamId);
            throw;
        }
    }

    public async Task<bool> HasPlayerFromSameTeamAsync(int userTeamId, int realTeamId, CancellationToken cancellationToken = default)
    {
        try
        {
            var playersFromTeam = await _unitOfWork.Repository<UserTeamPlayer>()
                .FindAsync(utp => utp.UserTeamId == userTeamId, cancellationToken, utp => utp.Player);

            return playersFromTeam.Any(utp => utp.Player?.TeamId == realTeamId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if team {UserTeamId} has player from real team {RealTeamId}", userTeamId, realTeamId);
            throw;
        }
    }

    #endregion
}