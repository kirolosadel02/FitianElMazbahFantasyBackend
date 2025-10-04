using Microsoft.EntityFrameworkCore;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Data.Configurations;

namespace FitianElMazbahFantasy.Data;

public class FitianElMazbahFantasyDbContext : DbContext
{
    public FitianElMazbahFantasyDbContext(DbContextOptions<FitianElMazbahFantasyDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<UserTeam> UserTeams { get; set; }
    public DbSet<UserTeamPlayer> UserTeamPlayers { get; set; }
    public DbSet<Fixture> Fixtures { get; set; }
    public DbSet<MatchResult> MatchResults { get; set; }
    public DbSet<MatchEvent> MatchEvents { get; set; }
    public DbSet<LeagueStanding> LeagueStandings { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Matchweek> Matchweeks { get; set; }
    public DbSet<UserTeamMatchweekPoints> UserTeamMatchweekPoints { get; set; }
    public DbSet<UserTeamSnapshot> UserTeamSnapshots { get; set; }
    public DbSet<UserTeamSnapshotPlayer> UserTeamSnapshotPlayers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new UserTeamConfiguration());
        modelBuilder.ApplyConfiguration(new UserTeamPlayerConfiguration());
        modelBuilder.ApplyConfiguration(new FixtureConfiguration());
        modelBuilder.ApplyConfiguration(new MatchResultConfiguration());
        modelBuilder.ApplyConfiguration(new MatchEventConfiguration());
        modelBuilder.ApplyConfiguration(new LeagueStandingConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new MatchweekConfiguration());
        modelBuilder.ApplyConfiguration(new UserTeamMatchweekPointsConfiguration());
        modelBuilder.ApplyConfiguration(new UserTeamSnapshotConfiguration());
        modelBuilder.ApplyConfiguration(new UserTeamSnapshotPlayerConfiguration());
    }
}