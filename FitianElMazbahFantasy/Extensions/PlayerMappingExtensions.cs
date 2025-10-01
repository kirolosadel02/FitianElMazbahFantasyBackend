using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.DTOs.Player;

namespace FitianElMazbahFantasy.Extensions;

public static class PlayerMappingExtensions
{
    public static PlayerDto ToDto(this Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            Position = player.Position.ToString(),
            TeamId = player.TeamId,
            TeamName = player.Team?.Name ?? string.Empty,
            TeamLogoUrl = player.Team?.LogoUrl,
            CreatedAt = player.CreatedAt,
            UpdatedAt = player.UpdatedAt
        };
    }

    public static IEnumerable<PlayerDto> ToDto(this IEnumerable<Player> players)
    {
        return players.Select(p => p.ToDto());
    }

    public static Player ToEntity(this CreatePlayerDto createDto)
    {
        return new Player
        {
            Name = createDto.Name,
            Position = (PlayerPosition)createDto.Position,
            TeamId = createDto.TeamId
        };
    }

    public static void UpdateEntity(this UpdatePlayerDto updateDto, Player player)
    {
        player.Name = updateDto.Name;
        player.Position = (PlayerPosition)updateDto.Position;
        player.TeamId = updateDto.TeamId;
    }
}