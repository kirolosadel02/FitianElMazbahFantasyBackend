using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.Services.Interfaces;

public interface IMatchweekService
{
    Task<Matchweek?> GetCurrentMatchweekAsync(CancellationToken cancellationToken = default);
    Task<Matchweek?> GetActiveMatchweekAsync(CancellationToken cancellationToken = default);
    Task<Matchweek?> GetMatchweekByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Matchweek?> GetMatchweekByWeekNumberAsync(int weekNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Matchweek>> GetAllMatchweeksAsync(CancellationToken cancellationToken = default);
    Task<Matchweek> CreateMatchweekAsync(Matchweek matchweek, CancellationToken cancellationToken = default);
    Task<Matchweek> UpdateMatchweekAsync(Matchweek matchweek, CancellationToken cancellationToken = default);
    Task<bool> DeleteMatchweekAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IsDeadlinePassedAsync(int matchweekId, CancellationToken cancellationToken = default);
    Task<bool> CanModifyTeamsAsync(int matchweekId, CancellationToken cancellationToken = default);
    Task<UserTeamSnapshot> CreateTeamSnapshotAsync(int userTeamId, int matchweekId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserTeamSnapshot>> GetTeamSnapshotsForMatchweekAsync(int matchweekId, CancellationToken cancellationToken = default);
}