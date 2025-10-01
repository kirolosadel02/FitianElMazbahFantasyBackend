using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.DTOs.Player;

namespace FitianElMazbahFantasy.Services.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<Player>> GetAllPlayersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetFilteredPlayersAsync(PlayerFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<Player?> GetPlayerByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetPlayersByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetPlayersByPositionAsync(PlayerPosition position, CancellationToken cancellationToken = default);
    Task<Player> CreatePlayerAsync(Player player, CancellationToken cancellationToken = default);
    Task<Player> UpdatePlayerAsync(Player player, CancellationToken cancellationToken = default);
    Task<bool> DeletePlayerAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetTotalPlayersCountAsync(CancellationToken cancellationToken = default);
    Task<bool> PlayerExistsAsync(int id, CancellationToken cancellationToken = default);
}