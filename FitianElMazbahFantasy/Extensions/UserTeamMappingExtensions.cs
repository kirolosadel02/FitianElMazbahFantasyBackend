using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.DTOs.UserTeam;

namespace FitianElMazbahFantasy.Extensions;

public static class UserTeamMappingExtensions
{
    public static UserTeamDto ToDto(this UserTeam userTeam)
    {
        return new UserTeamDto
        {
            Id = userTeam.Id,
            UserId = userTeam.UserId,
            Username = userTeam.User?.Username ?? string.Empty,
            TeamName = userTeam.TeamName,
            TotalPoints = userTeam.TotalPoints,
            IsLocked = userTeam.IsLocked,
            CreatedAt = userTeam.CreatedAt,
            UpdatedAt = userTeam.UpdatedAt,
            PlayerCount = userTeam.UserTeamPlayers?.Count ?? 0
        };
    }

    public static UserTeamDetailsDto ToDetailsDto(this UserTeam userTeam)
    {
        return new UserTeamDetailsDto
        {
            Id = userTeam.Id,
            UserId = userTeam.UserId,
            Username = userTeam.User?.Username ?? string.Empty,
            TeamName = userTeam.TeamName,
            TotalPoints = userTeam.TotalPoints,
            IsLocked = userTeam.IsLocked,
            CreatedAt = userTeam.CreatedAt,
            UpdatedAt = userTeam.UpdatedAt,
            Players = userTeam.UserTeamPlayers?.Select(utp => new UserTeamPlayerDto
            {
                Id = utp.Id,
                PlayerId = utp.PlayerId,
                PlayerName = utp.Player?.Name ?? string.Empty,
                Position = utp.Player?.Position.ToString() ?? string.Empty,
                TeamName = utp.Player?.Team?.Name ?? string.Empty,
                AddedAt = utp.AddedAt
            }).ToList() ?? new List<UserTeamPlayerDto>()
        };
    }

    public static UserHasTeamDto ToHasTeamDto(this UserTeam? userTeam)
    {
        return new UserHasTeamDto
        {
            HasTeam = userTeam != null,
            Team = userTeam?.ToDto()
        };
    }
}