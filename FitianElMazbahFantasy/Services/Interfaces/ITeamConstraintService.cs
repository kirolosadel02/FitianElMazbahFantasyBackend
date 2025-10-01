using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Constants;

namespace FitianElMazbahFantasy.Services.Interfaces;

public interface ITeamConstraintService
{
    Task<TeamConstraintValidationResult> ValidatePlayerAdditionAsync(int userTeamId, int playerId, CancellationToken cancellationToken = default);
    Task<TeamConstraintValidationResult> ValidateTeamForLockingAsync(int userTeamId, CancellationToken cancellationToken = default);
    Task<TeamConstraintValidationResult> ValidateTeamCompositionAsync(int userTeamId, CancellationToken cancellationToken = default);
    Task<bool> CanAddPlayerAsync(int userTeamId, int playerId, CancellationToken cancellationToken = default);
    Task<bool> IsTeamReadyForLockingAsync(int userTeamId, CancellationToken cancellationToken = default);
    Task<TeamCompositionStats> GetTeamCompositionStatsAsync(int userTeamId, CancellationToken cancellationToken = default);
}

public class TeamConstraintValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public List<string> Violations { get; set; } = new List<string>();
    
    public TeamConstraintValidationResult(bool isValid, string errorMessage = "", List<string>? violations = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        Violations = violations ?? new List<string>();
    }
    
    public static TeamConstraintValidationResult Success() => new(true);
    public static TeamConstraintValidationResult Failure(string errorMessage, List<string>? violations = null) 
        => new(false, errorMessage, violations);
}

public class TeamCompositionStats
{
    public int TotalPlayers { get; set; }
    public int Goalkeepers { get; set; }
    public int Defenders { get; set; }
    public int Midfielders { get; set; }
    public int Forwards { get; set; }
    public List<int> RepresentedTeamIds { get; set; } = new List<int>();
    public bool HasDuplicateTeams { get; set; }
    public bool MeetsGoalkeeperRequirement { get; set; }
    public bool MeetsPlayerCountRequirement { get; set; }
    public bool MeetsUniqueTeamRequirement { get; set; }
    public bool IsValidForLocking { get; set; }
}