# FitianElMazbahFantasy Documentation

## Table of Contents
1. [System Overview](#system-overview)
2. [API Reference](#api-reference)
3. [Architecture](#architecture)
4. [Database Design](#database-design)
5. [Authentication & Authorization](#authentication--authorization)
6. [Business Rules](#business-rules)
7. [Development Guide](#development-guide)

---

## System Overview

FitianElMazbahFantasy is a fantasy football system built with .NET 8, featuring:
- **JWT-based authentication** with role-based authorization
- **Repository pattern** with Unit of Work
- **Fantasy team management** with business rule enforcement
- **Player selection system** without pricing constraints
- **RESTful API** with comprehensive validation

### Key Features
- User registration and authentication
- Fantasy team creation (one per user)
- Player selection with composition rules
- Team locking mechanism
- Admin management capabilities
- Real-time validation

---

## API Reference

### Authentication Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/users/register` | None | Register new user account |
| POST | `/api/users/login` | None | Authenticate user and get JWT token |
| POST | `/api/users/refresh-token` | None | Refresh JWT token |
| POST | `/api/users/logout` | Bearer | Revoke current device's refresh token |
| POST | `/api/users/logout-all` | Bearer | Revoke all refresh tokens for user |
| GET | `/api/users/profile` | Bearer | Get current user's profile |

### Player Management

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/players` | Bearer | Get all players (with filtering) |
| GET | `/api/players/{id}` | Bearer | Get specific player |
| GET | `/api/players/team/{teamId}` | Bearer | Get players by team |
| GET | `/api/players/position/{position}` | Bearer | Get players by position (1-4) |
| GET | `/api/players/count` | Bearer | Get total players count |
| POST | `/api/players` | Admin | Create new player |
| PUT | `/api/players/{id}` | Admin | Update player |
| DELETE | `/api/players/{id}` | Admin | Delete player |

#### Player Filtering
Query parameters for `/api/players`:
- `position`: 1=Goalkeeper, 2=Defender, 3=Midfielder, 4=Forward
- `teamId`: Filter by real football team
- `name`: Search by player name (partial match)
- `pageNumber` & `pageSize`: Pagination (default: page 1, size 20)

### Fantasy Team Management

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/userteams/my-team` | Bearer | Get current user's team |
| GET | `/api/userteams/my-team/details` | Bearer | Get team with player details |
| GET | `/api/userteams/{id}` | Bearer | Get team by ID (own/admin) |
| GET | `/api/userteams/{id}/details` | Bearer | Get team details by ID |
| GET | `/api/userteams/check-has-team` | Bearer | Check if user has a team |
| POST | `/api/userteams` | Bearer | Create fantasy team |
| PUT | `/api/userteams/{id}` | Bearer | Update team name (own/admin) |
| DELETE | `/api/userteams/{id}` | Bearer | Delete team (own/admin) |

### Player Selection

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/userteams/{teamId}/players/{playerId}` | Bearer | Add player to team |
| DELETE | `/api/userteams/{teamId}/players/{playerId}` | Bearer | Remove player from team |
| POST | `/api/userteams/{teamId}/lock` | Bearer | Lock team for matchweek |
| POST | `/api/userteams/{teamId}/unlock` | Admin | Unlock team (admin only) |

### Team Information

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/teams` | Bearer | Get all football teams |
| GET | `/api/teams/{id}` | Bearer | Get team with players |
| POST | `/api/teams` | Admin | Create new football team |
| PUT | `/api/teams/{id}` | Admin | Update football team |
| DELETE | `/api/teams/{id}` | Admin | Delete football team |

### User Management (Admin)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/users` | Admin | Get all users |
| GET | `/api/users/{id}` | Bearer | Get user by ID (own/admin) |
| DELETE | `/api/users/{id}` | Admin | Delete user account |

---

## Architecture

### Repository Pattern & Unit of Work

The system implements the Repository Pattern with Unit of Work for data access:

```csharp
// Service layer example
public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Player?> GetPlayerByIdAsync(int id)
    {
        return await _unitOfWork.Repository<Player>()
            .GetByIdAsync(id, p => p.Team);
    }
}
```

### Key Components

1. **Generic Repository**: `IGenericRepository<T>` with CRUD operations
2. **Unit of Work**: `IUnitOfWork` for transaction management
3. **Service Layer**: Business logic encapsulation
4. **DTOs**: Data transfer objects with validation
5. **Mapping Extensions**: Clean object transformation

### Service Registration
```csharp
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IPlayerService, PlayerService>();
services.AddScoped<IUserTeamService, UserTeamService>();
```

---

## Database Design

### Core Entities

**User** (One-to-One with UserTeam)
- Authentication and profile information
- Role-based access (User/Admin)

**UserTeam** (User's Fantasy Team)
- One team per user constraint
- Team locking mechanism
- Points tracking

**Player** (Real Football Players)
- No pricing system (removed for simplicity)
- Position-based (Goalkeeper, Defender, Midfielder, Forward)
- Team associations

**Team** (Real Football Teams)
- Team information and logos
- Player collections

**UserTeamPlayer** (Junction Table)
- Links fantasy teams to selected players
- Tracks selection dates

### Key Relationships
```
User (1) ?? (1) UserTeam (1) ?? (*) UserTeamPlayer (*) ?? (1) Player
Team (1) ?? (*) Player
```

### Database Constraints
- **Unique constraint**: One team per user (`UserTeam.UserId`)
- **Composite unique**: One player per team (`UserTeamPlayer.UserTeamId, PlayerId`)
- **Check constraints**: Business rule enforcement

---

## Authentication & Authorization

### JWT Configuration
```json
{
  "JwtSettings": {
    "SecretKey": "YourSecretKeyHere",
    "Issuer": "FitianElMazbahFantasy",
    "Audience": "FitianElMazbahFantasyUsers",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

### Token Structure
**Access Token Claims:**
- `NameIdentifier`: User ID
- `Role`: User Role (Admin/User)
- Custom claims for application logic

**Refresh Token:**
- Secure random string stored in database
- Individual and bulk revocation support

### Authorization Levels
- **Public**: Registration, Login
- **User**: Own team management, player browsing
- **Admin**: All operations, user management

### Admin User Creation
Admin users must be created manually (cannot register via API):
```sql
INSERT INTO Users (Username, Email, Password, Role, CreatedAt)
VALUES ('admin', 'admin@example.com', 'HashedPassword', 2, GETUTCDATE());
```

---

## Business Rules

### Fantasy Team Composition
1. **Maximum 4 players** per fantasy team
2. **Exactly 1 goalkeeper** required
3. **Players from different real teams** (no duplicates)
4. **One team per user** constraint

### Team Management
- **Team Creation**: Unique name per user
- **Player Selection**: Real-time validation
- **Team Locking**: Prevents changes after deadline
- **Admin Override**: Admins can unlock any team

### Validation Points
- **Add Player**: Validates all composition rules
- **Team Lock**: Ensures complete team before locking
- **Player Removal**: Allowed only when team unlocked

---

## Development Guide

### Request/Response Examples

#### Create Fantasy Team
```json
POST /api/userteams
{
    "teamName": "My Dream Team"
}
```

#### Get Team Details
```json
GET /api/userteams/my-team/details

Response:
{
    "id": 1,
    "teamName": "My Dream Team",
    "totalPoints": 245,
    "isLocked": false,
    "players": [
        {
            "playerId": 25,
            "playerName": "Mohamed Salah",
            "position": "Forward",
            "teamName": "Liverpool",
            "addedAt": "2024-01-01T11:00:00Z"
        }
    ]
}
```

#### Filter Players
```json
GET /api/players?position=4&pageNumber=1&pageSize=10

Response:
[
    {
        "id": 25,
        "name": "Mohamed Salah",
        "position": "Forward",
        "teamName": "Liverpool",
        "teamLogoUrl": "https://example.com/logo.png"
    }
]
```

### Error Handling

#### Common Error Responses
```json
// 400 Bad Request
{
    "message": "Team cannot have more than 4 players"
}

// 403 Forbidden
{
    "message": "Access denied"
}

// 404 Not Found
{
    "message": "Team not found"
}
```

### Status Code Summary
- **200**: Successful GET/PUT requests
- **201**: Successful POST (creation)
- **204**: Successful DELETE
- **400**: Validation errors, business rule violations
- **401**: Missing/invalid JWT token
- **403**: Insufficient permissions
- **404**: Resource doesn't exist
- **500**: Server-side errors

### Testing with JWT
Include JWT token in Authorization header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

This documentation provides a complete reference for the FitianElMazbahFantasy fantasy football system with proper authentication, authorization, and business rule enforcement.