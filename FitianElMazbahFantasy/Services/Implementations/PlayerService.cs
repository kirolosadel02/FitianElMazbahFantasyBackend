using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.DTOs.Player;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.Services.Interfaces;

namespace FitianElMazbahFantasy.Services.Implementations;

public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(IUnitOfWork unitOfWork, ILogger<PlayerService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Player>> GetAllPlayersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Player>().GetAllAsync(cancellationToken, 
                p => p.Team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all players");
            throw;
        }
    }

    public async Task<IEnumerable<Player>> GetFilteredPlayersAsync(PlayerFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var players = await _unitOfWork.Repository<Player>().GetAllAsync(cancellationToken, 
                p => p.Team);

            // Apply filters
            var query = players.AsQueryable();

            if (filterDto.Position.HasValue)
            {
                query = query.Where(p => (int)p.Position == filterDto.Position.Value);
            }

            if (filterDto.TeamId.HasValue)
            {
                query = query.Where(p => p.TeamId == filterDto.TeamId.Value);
            }

            if (!string.IsNullOrEmpty(filterDto.Name))
            {
                query = query.Where(p => p.Name.Contains(filterDto.Name, StringComparison.OrdinalIgnoreCase));
            }

            // Apply pagination
            var skip = (filterDto.PageNumber - 1) * filterDto.PageSize;
            return query.Skip(skip).Take(filterDto.PageSize).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered players");
            throw;
        }
    }

    public async Task<Player?> GetPlayerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Player>().GetByIdAsync(id, cancellationToken, 
                p => p.Team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player by id {PlayerId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Player>> GetPlayersByTeamIdAsync(int teamId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Player>().FindAsync(
                p => p.TeamId == teamId, 
                cancellationToken,
                p => p.Team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players by team id {TeamId}", teamId);
            throw;
        }
    }

    public async Task<IEnumerable<Player>> GetPlayersByPositionAsync(PlayerPosition position, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Player>().FindAsync(
                p => p.Position == position, 
                cancellationToken,
                p => p.Team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players by position {Position}", position);
            throw;
        }
    }

    public async Task<Player> CreatePlayerAsync(Player player, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate team exists
            var teamExists = await _unitOfWork.Repository<Team>().AnyAsync(t => t.Id == player.TeamId, cancellationToken);
            if (!teamExists)
            {
                throw new InvalidOperationException($"Team with ID {player.TeamId} does not exist");
            }

            player.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Player>().AddAsync(player, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return the created player with team info
            var createdPlayer = await GetPlayerByIdAsync(player.Id, cancellationToken);

            _logger.LogInformation("Player created successfully with id {PlayerId}", player.Id);
            return createdPlayer!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating player {PlayerName}", player.Name);
            throw;
        }
    }

    public async Task<Player> UpdatePlayerAsync(Player player, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate team exists
            var teamExists = await _unitOfWork.Repository<Team>().AnyAsync(t => t.Id == player.TeamId, cancellationToken);
            if (!teamExists)
            {
                throw new InvalidOperationException($"Team with ID {player.TeamId} does not exist");
            }

            player.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Player>().Update(player);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return the updated player with team info
            var updatedPlayer = await GetPlayerByIdAsync(player.Id, cancellationToken);

            _logger.LogInformation("Player updated successfully with id {PlayerId}", player.Id);
            return updatedPlayer!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player with id {PlayerId}", player.Id);
            throw;
        }
    }

    public async Task<bool> DeletePlayerAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var player = await _unitOfWork.Repository<Player>().GetByIdAsync(id, cancellationToken);
            if (player == null)
            {
                return false;
            }

            // Check if player is in any user teams
            var isInUserTeam = await _unitOfWork.Repository<UserTeamPlayer>().AnyAsync(
                utp => utp.PlayerId == id, cancellationToken);
            
            if (isInUserTeam)
            {
                throw new InvalidOperationException("Cannot delete player as they are currently selected in user teams");
            }

            _unitOfWork.Repository<Player>().Remove(player);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Player deleted successfully with id {PlayerId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting player with id {PlayerId}", id);
            throw;
        }
    }

    public async Task<int> GetTotalPlayersCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Player>().CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total players count");
            throw;
        }
    }

    public async Task<bool> PlayerExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Player>().AnyAsync(p => p.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if player exists with id {PlayerId}", id);
            throw;
        }
    }
}