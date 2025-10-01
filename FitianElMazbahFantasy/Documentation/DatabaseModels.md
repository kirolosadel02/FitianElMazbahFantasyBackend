# FitianElMazbahFantasy Database Models

## Models Created

### Core User Management
- **User**: Stores user information with roles (Admin/User)
- **UserTeam**: Represents a fantasy team created by a user (one-to-one relationship with User)
- **UserTeamPlayer**: Junction table linking users' teams to their selected players

### Football Data
- **Team**: Real football teams
- **Player**: Real football players with positions (no pricing system)
- **Fixture**: Match fixtures between two teams
- **MatchResult**: Separate table for match results (score format like "2-1")
- **MatchEvent**: Individual player events in matches (goals, assists, etc.)

### Leaderboards
- **LeagueStanding**: Weekly standings/leaderboard positions

## Key Design Decisions

### 1. User Roles
- Added `UserRole` enum with User and Admin roles
- Admin users can manage teams, players, and match results
- Regular users can create fantasy teams and compete

### 2. One Team Per User Constraint
- **UPDATED**: Changed from one-to-many to one-to-one relationship between User and UserTeam
- Each user can have only one fantasy team
- Database-level unique constraint on `UserTeam.UserId`
- Business logic validation prevents multiple team creation attempts

### 3. Match Results Architecture
- Separated `Fixture` (match scheduling) from `MatchResult` (actual scores)
- `MatchResult.FinalScore` stores formatted score like "2-1"
- Also stores individual team scores for calculations
- No home/away distinction as requested

### 4. Fantasy Team Rules Enforced
- Maximum 4 players per user team
- Exactly 1 goalkeeper required
- All players must be from different real teams
- Team locking mechanism for deadline enforcement
- **No pricing system** - player selection based purely on strategy

### 5. Scoring System
- `MatchEvent` table tracks all scoring events
- Flexible point system defined in `ScoringConstants`
- Events: Goals (+5), Assists (+3), Clean Sheets (+4), Cards (negative points)

### 6. Database Best Practices
- Proper normalization (3NF)
- Appropriate indexes for performance
- Foreign key constraints with proper delete behavior
- Check constraints for business rules
- Audit fields (CreatedAt, UpdatedAt) on all entities

## Entity Relationships

```
User (1) -> (1) UserTeam (1) -> (*) UserTeamPlayer (*) <- (1) Player
Team (1) -> (*) Player
Team (1) -> (*) Fixture (as Team1)
Team (1) -> (*) Fixture (as Team2)
Fixture (1) -> (0..1) MatchResult
Fixture (1) -> (*) MatchEvent
Player (1) -> (*) MatchEvent
UserTeam (1) -> (*) LeagueStanding
```

## Ready for Migration
All models are configured with Entity Framework Fluent API and ready for database migration.

## Breaking Changes
- **User.UserTeams** changed to **User.UserTeam** (one-to-one relationship)
- Added unique constraint on `UserTeam.UserId`
- Updated API endpoints: `/users/{id}/teams` deprecated in favor of `/users/{id}/team`
- **Removed player pricing system** - players no longer have monetary values