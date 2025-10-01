# Database Models & Design

## Entity Overview

### Core User Management
- **User**: Authentication and profile (roles: Admin/User)
- **UserTeam**: Fantasy team (one-to-one with User)
- **UserTeamPlayer**: Junction table for player selections

### Matchweek System (NEW)
- **Matchweek**: Week-based fixture grouping with deadlines
- **UserTeamMatchweekPoints**: Detailed points tracking per matchweek
- **UserTeamSnapshot**: Team composition snapshots at deadlines
- **UserTeamSnapshotPlayer**: Player details within snapshots

### Football Data
- **Team**: Real football teams with logos
- **Player**: Real players with positions (no pricing system)
- **Fixture**: Match scheduling between teams (now linked to Matchweek)
- **MatchResult**: Match outcomes and scores
- **MatchEvent**: Player events (goals, assists, cards, etc.)

### Leaderboards
- **LeagueStanding**: Weekly rankings and points

## Key Entity Relationships

```
User (1) ?? (1) UserTeam (1) ?? (*) UserTeamPlayer (*) ?? (1) Player
Team (1) ?? (*) Player
Team (1) ?? (*) Fixture (as Team1 and Team2)
Matchweek (1) ?? (*) Fixture
Matchweek (1) ?? (*) UserTeamMatchweekPoints
Matchweek (1) ?? (*) UserTeamSnapshot
UserTeam (1) ?? (*) UserTeamMatchweekPoints
UserTeam (1) ?? (*) UserTeamSnapshot
UserTeamSnapshot (1) ?? (*) UserTeamSnapshotPlayer
Fixture (1) ?? (0..1) MatchResult
Fixture (1) ?? (*) MatchEvent
Player (1) ?? (*) MatchEvent
UserTeam (1) ?? (*) LeagueStanding
```

## Database Constraints

### Business Rule Constraints
- **Unique Index**: `UserTeam.UserId` (one team per user)
- **Composite Unique**: `UserTeamPlayer(UserTeamId, PlayerId)` (no duplicate players)
- **Composite Unique**: `UserTeamSnapshot(UserTeamId, MatchweekId)` (one snapshot per team per matchweek)
- **Composite Primary**: `UserTeamMatchweekPoints(UserTeamId, MatchweekId)`
- **Check Constraint**: `Fixture.Team1Id <> Team2Id` (team can't play itself)
- **Unique Index**: `Matchweek.WeekNumber` (unique week numbers)

### Performance Indexes
- **Player**: Position, TeamId
- **Fixture**: MatchWeek, MatchweekId, MatchDate, IsCompleted
- **MatchEvent**: FixtureId, PlayerId, EventType
- **LeagueStanding**: MatchWeek, Position (unique composite)
- **Matchweek**: WeekNumber (unique), DeadlineDate, IsActive, IsCompleted
- **UserTeamMatchweekPoints**: UserTeamId, MatchweekId, Points
- **UserTeamSnapshot**: UserTeamId, MatchweekId (unique composite)

## Key Design Decisions

### 1. One Team Per User
- Changed from one-to-many to one-to-one relationship
- Database-level unique constraint on `UserTeam.UserId`
- Business logic validation prevents multiple team creation

### 2. No Player Pricing System
- Simplified team selection based on strategy, not budget
- Removes complexity of price management and budget calculations
- Players selected based purely on performance expectations

### 3. Enhanced Matchweek Structure (NEW)
- **Matchweek Entity**: Groups fixtures and defines deadlines
- **Deadline Management**: Prevents team modifications after deadline
- **Team Snapshots**: Preserves team selections at deadline time
- **Historical Tracking**: Complete audit trail of team changes

### 4. Advanced Points Tracking (NEW)
- **Per-Matchweek Points**: Detailed breakdown by week
- **Statistical Tracking**: Goals, assists, cards, saves, etc.
- **Performance Analytics**: Enables detailed reporting

### 5. Fantasy Team Rules Enforcement (ENHANCED)
- Maximum 4 players per team (validated in real-time)
- Exactly 1 goalkeeper required (database + application level)
- All players from different real teams (comprehensive checking)
- Team locking mechanism with snapshot creation
- Enhanced constraint validation service

## Entity Configurations

### Matchweek Configuration (NEW)
```csharp
builder.HasIndex(m => m.WeekNumber).IsUnique(); // Unique week numbers
builder.Property(m => m.IsActive).HasDefaultValue(false);
builder.Property(m => m.IsCompleted).HasDefaultValue(false);
```

### UserTeamMatchweekPoints Configuration (NEW)
```csharp
builder.HasKey(utmp => new { utmp.UserTeamId, utmp.MatchweekId }); // Composite key
builder.Property(utmp => utmp.Points).HasDefaultValue(0);
// ... all stat fields with default values
```

### UserTeamSnapshot Configuration (NEW)
```csharp
builder.HasIndex(uts => new { uts.UserTeamId, uts.MatchweekId }).IsUnique();
// One snapshot per team per matchweek
```

### Enhanced Fixture Configuration
```csharp
builder.Property(f => f.MatchweekId).IsRequired(); // Link to Matchweek
builder.HasOne(f => f.Matchweek)
    .WithMany(m => m.Fixtures)
    .HasForeignKey(f => f.MatchweekId);
```

### UserTeam Configuration (UPDATED)
```csharp
builder.HasIndex(ut => ut.UserId).IsUnique(); // One team per user
builder.Property(ut => ut.TotalPoints).HasDefaultValue(0);
builder.Property(ut => ut.IsLocked).HasDefaultValue(false);

// Enhanced navigation properties
builder.HasMany(ut => ut.MatchweekPoints)
    .WithOne(utmp => utmp.UserTeam);
builder.HasMany(ut => ut.TeamSnapshots)
    .WithOne(uts => uts.UserTeam);
```

## Scoring System Constants

```csharp
public static class ScoringConstants
{
    public const int GoalPoints = 5;
    public const int AssistPoints = 3;
    public const int CleanSheetPoints = 4;
    public const int YellowCardPoints = -1;
    public const int RedCardPoints = -3;
    public const int SavePoints = 1;
    public const int PenaltyPoints = 6;
}

public static class GameRules
{
    public const int MaxPlayersPerTeam = 4;
    public const int MinGoalkeepers = 1;
    public const int MaxGoalkeepers = 1;
    public const int TransfersPerWeek = 1;
}
```

## Enhanced Constraint Validation

### TeamConstraintService Features
- **Real-time validation** during player addition
- **Comprehensive team composition checking**
- **Detailed violation reporting**
- **Team readiness assessment for locking**

### Validation Rules Enforced
1. **Player Count**: Maximum 4 players per team
2. **Goalkeeper Requirement**: Exactly 1 goalkeeper
3. **Team Diversity**: All players from different real teams
4. **Duplicate Prevention**: No same player twice in team
5. **Deadline Respect**: No modifications after matchweek deadline

## Migration Status

All models are configured with Entity Framework Fluent API and ready for database migration. The system now enforces business rules at multiple levels:
- **Database Level**: Constraints and indexes
- **Application Level**: Service validation
- **API Level**: Request validation
- **Business Logic Level**: Comprehensive rule checking

### Required Migration
```bash
dotnet ef migrations add AddMatchweekAndEnhancedConstraints
dotnet ef database update
```

This enhanced database design provides a robust foundation for a professional fantasy football system with proper constraint enforcement, historical tracking, and deadline management.