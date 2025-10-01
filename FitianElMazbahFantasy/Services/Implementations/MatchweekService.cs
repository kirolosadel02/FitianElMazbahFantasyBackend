using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.Services.Interfaces;

namespace FitianElMazbahFantasy.Services.Implementations;

public class MatchweekService : IMatchweekService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MatchweekService> _logger;

    public MatchweekService(IUnitOfWork unitOfWork, ILogger<MatchweekService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Matchweek?> GetCurrentMatchweekAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            return await _unitOfWork.Repository<Matchweek>()
                .FirstOrDefaultAsync(m => m.DeadlineDate >= now && !m.IsCompleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current matchweek");
            throw;
        }
    }

    public async Task<Matchweek?> GetActiveMatchweekAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Matchweek>()
                .FirstOrDefaultAsync(m => m.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active matchweek");
            throw;
        }
    }

    public async Task<Matchweek?> GetMatchweekByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Matchweek>().GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matchweek by id {MatchweekId}", id);
            throw;
        }
    }

    public async Task<Matchweek?> GetMatchweekByWeekNumberAsync(int weekNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Matchweek>()
                .FirstOrDefaultAsync(m => m.WeekNumber == weekNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matchweek by week number {WeekNumber}", weekNumber);
            throw;
        }
    }

    public async Task<IEnumerable<Matchweek>> GetAllMatchweeksAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<Matchweek>().GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all matchweeks");
            throw;
        }
    }

    public async Task<Matchweek> CreateMatchweekAsync(Matchweek matchweek, CancellationToken cancellationToken = default)
    {
        try
        {
            matchweek.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Matchweek>().AddAsync(matchweek, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Matchweek created successfully with id {MatchweekId}", matchweek.Id);
            return matchweek;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating matchweek");
            throw;
        }
    }

    public async Task<Matchweek> UpdateMatchweekAsync(Matchweek matchweek, CancellationToken cancellationToken = default)
    {
        try
        {
            matchweek.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Matchweek>().Update(matchweek);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Matchweek updated successfully with id {MatchweekId}", matchweek.Id);
            return matchweek;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating matchweek with id {MatchweekId}", matchweek.Id);
            throw;
        }
    }

    public async Task<bool> DeleteMatchweekAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var matchweek = await _unitOfWork.Repository<Matchweek>().GetByIdAsync(id, cancellationToken);
            if (matchweek == null)
            {
                return false;
            }

            _unitOfWork.Repository<Matchweek>().Remove(matchweek);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Matchweek deleted successfully with id {MatchweekId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting matchweek with id {MatchweekId}", id);
            throw;
        }
    }

    public async Task<bool> IsDeadlinePassedAsync(int matchweekId, CancellationToken cancellationToken = default)
    {
        try
        {
            var matchweek = await GetMatchweekByIdAsync(matchweekId, cancellationToken);
            if (matchweek == null)
            {
                return true; // If matchweek doesn't exist, consider deadline passed
            }

            return DateTime.UtcNow > matchweek.DeadlineDate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if deadline passed for matchweek {MatchweekId}", matchweekId);
            throw;
        }
    }

    public async Task<bool> CanModifyTeamsAsync(int matchweekId, CancellationToken cancellationToken = default)
    {
        try
        {
            var isDeadlinePassed = await IsDeadlinePassedAsync(matchweekId, cancellationToken);
            return !isDeadlinePassed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if teams can be modified for matchweek {MatchweekId}", matchweekId);
            throw;
        }
    }

    public async Task<UserTeamSnapshot> CreateTeamSnapshotAsync(int userTeamId, int matchweekId, CancellationToken cancellationToken = default)
    {
        // Early return if snapshot already exists (avoid transaction)
        var existingSnapshot = await _unitOfWork.Repository<UserTeamSnapshot>()
            .FirstOrDefaultAsync(uts => uts.UserTeamId == userTeamId && uts.MatchweekId == matchweekId, cancellationToken);
        if (existingSnapshot != null)
        {
            return existingSnapshot;
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var userTeam = await _unitOfWork.Repository<UserTeam>()
                .GetByIdAsync(userTeamId, cancellationToken, ut => ut.UserTeamPlayers);

            if (userTeam == null)
            {
                throw new InvalidOperationException($"User team with ID {userTeamId} not found");
            }

            var snapshot = new UserTeamSnapshot
            {
                UserTeamId = userTeamId,
                MatchweekId = matchweekId,
                TeamName = userTeam.TeamName,
                SnapshotDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<UserTeamSnapshot>().AddAsync(snapshot, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var players = await _unitOfWork.Repository<UserTeamPlayer>()
                .FindAsync(utp => utp.UserTeamId == userTeamId, cancellationToken, utp => utp.Player, utp => utp.Player.Team);

            foreach (var utp in players)
            {
                if (utp.Player != null)
                {
                    var playerSnapshot = new UserTeamSnapshotPlayer
                    {
                        SnapshotId = snapshot.Id,
                        PlayerId = utp.PlayerId,
                        PlayerName = utp.Player.Name,
                        Position = utp.Player.Position,
                        TeamName = utp.Player.Team?.Name ?? string.Empty,
                        AddedAt = utp.AddedAt
                    };

                    await _unitOfWork.Repository<UserTeamSnapshotPlayer>().AddAsync(playerSnapshot, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Team snapshot created for user team {UserTeamId} in matchweek {MatchweekId}", userTeamId, matchweekId);
            return snapshot;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error creating team snapshot for user team {UserTeamId} in matchweek {MatchweekId}", userTeamId, matchweekId);
            throw;
        }
    }

    public async Task<IEnumerable<UserTeamSnapshot>> GetTeamSnapshotsForMatchweekAsync(int matchweekId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserTeamSnapshot>()
                .FindAsync(uts => uts.MatchweekId == matchweekId, cancellationToken,
                    uts => uts.UserTeam,
                    uts => uts.PlayerSnapshots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team snapshots for matchweek {MatchweekId}", matchweekId);
            throw;
        }
    }
}