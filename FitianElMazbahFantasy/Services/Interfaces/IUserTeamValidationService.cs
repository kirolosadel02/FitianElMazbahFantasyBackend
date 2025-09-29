using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Services.Interfaces;

public interface IUserTeamValidationService
{
    Task<bool> ValidateTeamComposition(List<int> playerIds);
    Task<bool> HasOneGoalkeeper(List<int> playerIds);
    Task<bool> AllPlayersFromDifferentTeams(List<int> playerIds);
    Task<bool> HasExactlyFourPlayers(List<int> playerIds);
    Task<bool> CanMakeTransfer(int userTeamId, int currentMatchWeek);
}

public class TeamValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}