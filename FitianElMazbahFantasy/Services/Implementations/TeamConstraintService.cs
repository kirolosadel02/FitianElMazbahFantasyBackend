using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Constants;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.Services.Interfaces;

namespace FitianElMazbahFantasy.Services.Implementations;

public class TeamConstraintService : ITeamConstraintService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TeamConstraintService> _logger;

    public TeamConstraintService(IUnitOfWork unitOfWork, ILogger<TeamConstraintService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TeamConstraintValidationResult> ValidatePlayerAdditionAsync(int userTeamId, int playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var violations = new List<string>();
            
            // Get current team composition
            var stats = await GetTeamCompositionStatsAsync(userTeamId, cancellationToken);
            
            // Get the player being added
            var player = await _unitOfWork.Repository<Player>().GetByIdAsync(playerId, cancellationToken, p => p.Team);
            if (player == null)
            {
                return TeamConstraintValidationResult.Failure("Player not found");
            }

            // Check max players rule
            if (stats.TotalPlayers >= GameRules.MaxPlayersPerTeam)
            {
                violations.Add($"Team cannot have more than {GameRules.MaxPlayersPerTeam} players");
            }

            // Check goalkeeper limits
            if (player.Position == PlayerPosition.Goalkeeper && stats.Goalkeepers >= GameRules.MaxGoalkeepers)
            {
                violations.Add($"Team cannot have more than {GameRules.MaxGoalkeepers} goalkeeper");
            }

            // Check unique team constraint
            if (stats.RepresentedTeamIds.Contains(player.TeamId))
            {
                violations.Add("All players must be from different teams");
            }

            // Check if player is already in team
            var isPlayerInTeam = await _unitOfWork.Repository<UserTeamPlayer>()
                .AnyAsync(utp => utp.UserTeamId == userTeamId && utp.PlayerId == playerId, cancellationToken);
            
            if (isPlayerInTeam)
            {
                violations.Add("Player is already in your team");
            }

            if (violations.Any())
            {
                return TeamConstraintValidationResult.Failure(violations.First(), violations);
            }

            return TeamConstraintValidationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating player addition for team {UserTeamId}, player {PlayerId}", userTeamId, playerId);
            return TeamConstraintValidationResult.Failure("Error validating player addition");
        }
    }

    public async Task<TeamConstraintValidationResult> ValidateTeamForLockingAsync(int userTeamId, CancellationToken cancellationToken = default)
    {
        try
        {
            var violations = new List<string>();
            var stats = await GetTeamCompositionStatsAsync(userTeamId, cancellationToken);

            // Check player count requirement
            if (stats.TotalPlayers != GameRules.MaxPlayersPerTeam)
            {
                violations.Add($"Team must have exactly {GameRules.MaxPlayersPerTeam} players before locking");
            }

            // Check goalkeeper requirement
            if (stats.Goalkeepers != GameRules.MinGoalkeepers)
            {
                violations.Add($"Team must have exactly {GameRules.MinGoalkeepers} goalkeeper");
            }

            // Check unique team requirement
            if (stats.HasDuplicateTeams)
            {
                violations.Add("All players must be from different teams");
            }

            if (violations.Any())
            {
                return TeamConstraintValidationResult.Failure(violations.First(), violations);
            }

            return TeamConstraintValidationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating team for locking {UserTeamId}", userTeamId);
            return TeamConstraintValidationResult.Failure("Error validating team for locking");
        }
    }

    public async Task<TeamConstraintValidationResult> ValidateTeamCompositionAsync(int userTeamId, CancellationToken cancellationToken = default)
    {
        try
        {
            var violations = new List<string>();
            var stats = await GetTeamCompositionStatsAsync(userTeamId, cancellationToken);

            // Check maximum players
            if (stats.TotalPlayers > GameRules.MaxPlayersPerTeam)
            {
                violations.Add($"Team cannot have more than {GameRules.MaxPlayersPerTeam} players");
            }

            // Check goalkeeper limits
            if (stats.Goalkeepers > GameRules.MaxGoalkeepers)
            {
                violations.Add($"Team cannot have more than {GameRules.MaxGoalkeepers} goalkeeper");
            }

            // Check unique team constraint
            if (stats.HasDuplicateTeams)
            {
                violations.Add("All players must be from different teams");
            }

            if (violations.Any())
            {
                return TeamConstraintValidationResult.Failure(violations.First(), violations);
            }

            return TeamConstraintValidationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating team composition {UserTeamId}", userTeamId);
            return TeamConstraintValidationResult.Failure("Error validating team composition");
        }
    }

    public async Task<bool> CanAddPlayerAsync(int userTeamId, int playerId, CancellationToken cancellationToken = default)
    {
        var result = await ValidatePlayerAdditionAsync(userTeamId, playerId, cancellationToken);
        return result.IsValid;
    }

    public async Task<bool> IsTeamReadyForLockingAsync(int userTeamId, CancellationToken cancellationToken = default)
    {
        var result = await ValidateTeamForLockingAsync(userTeamId, cancellationToken);
        return result.IsValid;
    }

    public async Task<TeamCompositionStats> GetTeamCompositionStatsAsync(int userTeamId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userTeamPlayers = await _unitOfWork.Repository<UserTeamPlayer>()
                .FindAsync(utp => utp.UserTeamId == userTeamId, cancellationToken, utp => utp.Player, utp => utp.Player.Team);

            var stats = new TeamCompositionStats
            {
                TotalPlayers = userTeamPlayers.Count()
            };

            if (stats.TotalPlayers == 0)
            {
                stats.MeetsGoalkeeperRequirement = false;
                stats.MeetsPlayerCountRequirement = false;
                stats.MeetsUniqueTeamRequirement = true;
                stats.IsValidForLocking = false;
                return stats;
            }

            // Count by position
            foreach (var utp in userTeamPlayers)
            {
                switch (utp.Player?.Position)
                {
                    case PlayerPosition.Goalkeeper:
                        stats.Goalkeepers++;
                        break;
                    case PlayerPosition.Defender:
                        stats.Defenders++;
                        break;
                    case PlayerPosition.Midfielder:
                        stats.Midfielders++;
                        break;
                    case PlayerPosition.Forward:
                        stats.Forwards++;
                        break;
                }

                if (utp.Player?.TeamId != null)
                {
                    stats.RepresentedTeamIds.Add(utp.Player.TeamId);
                }
            }

            // Check for duplicates
            stats.HasDuplicateTeams = stats.RepresentedTeamIds.Count != stats.RepresentedTeamIds.Distinct().Count();

            // Evaluate requirements
            stats.MeetsGoalkeeperRequirement = stats.Goalkeepers >= GameRules.MinGoalkeepers && stats.Goalkeepers <= GameRules.MaxGoalkeepers;
            stats.MeetsPlayerCountRequirement = stats.TotalPlayers <= GameRules.MaxPlayersPerTeam;
            stats.MeetsUniqueTeamRequirement = !stats.HasDuplicateTeams;
            stats.IsValidForLocking = stats.TotalPlayers == GameRules.MaxPlayersPerTeam && 
                                      stats.MeetsGoalkeeperRequirement && 
                                      stats.MeetsUniqueTeamRequirement;

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team composition stats for team {UserTeamId}", userTeamId);
            throw;
        }
    }
}