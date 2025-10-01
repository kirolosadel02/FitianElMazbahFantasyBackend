# Repository Pattern & Unit of Work

## Architecture Overview

The system implements Repository Pattern with Unit of Work for clean data access abstraction over Entity Framework Core.

### Key Components

1. **Generic Repository**: `IGenericRepository<T>` - Reusable CRUD operations
2. **Unit of Work**: `IUnitOfWork` - Transaction management and repository factory
3. **Service Layer**: Business logic encapsulation with repository orchestration

## Core Interfaces

### Generic Repository
```csharp
public interface IGenericRepository<T> where T : class
{
    // Basic operations
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // With eager loading
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default, 
        params Expression<Func<T, object>>[] includes);
    
    // Querying
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default);
    
    // Pagination
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, 
        CancellationToken cancellationToken = default);
    
    // CRUD operations
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}
```

### Unit of Work
```csharp
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

## Implementation Highlights

### Repository Factory Pattern
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories;
    
    public IGenericRepository<T> Repository<T>() where T : class
    {
        return (IGenericRepository<T>)_repositories.GetOrAdd(typeof(T), 
            _ => new GenericRepository<T>(_context));
    }
}
```

### Service Layer Usage
```csharp
public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Player?> GetPlayerByIdAsync(int id)
    {
        return await _unitOfWork.Repository<Player>()
            .GetByIdAsync(id, p => p.Team); // With eager loading
    }
    
    public async Task<Player> CreatePlayerAsync(Player player)
    {
        await _unitOfWork.Repository<Player>().AddAsync(player);
        await _unitOfWork.SaveChangesAsync();
        return player;
    }
}
```

### Transaction Management
```csharp
public async Task<UserTeam> CreateUserTeamWithPlayersAsync(UserTeam team, List<int> playerIds)
{
    await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        // Create team
        await _unitOfWork.Repository<UserTeam>().AddAsync(team);
        await _unitOfWork.SaveChangesAsync();
        
        // Add players
        foreach(var playerId in playerIds)
        {
            var userTeamPlayer = new UserTeamPlayer 
            { 
                UserTeamId = team.Id, 
                PlayerId = playerId,
                AddedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<UserTeamPlayer>().AddAsync(userTeamPlayer);
        }
        
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();
        
        return team;
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

## Benefits

### 1. Testability
- Easy to mock repositories for unit testing
- Clear separation between data access and business logic
- Dependency injection friendly

### 2. Performance
- ConcurrentDictionary for thread-safe repository caching
- Efficient eager loading with includes
- Proper async/await patterns with CancellationToken support

### 3. Maintainability
- Single responsibility principle
- Consistent API across all entities
- Centralized transaction management

### 4. Flexibility
- Generic implementation reduces code duplication
- Easy to extend with specific repositories when needed
- Support for complex queries and filtering

## Service Registration

```csharp
// In ServiceCollectionExtensions.cs
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IPlayerService, PlayerService>();
services.AddScoped<IUserTeamService, UserTeamService>();
```

This implementation provides a robust, testable foundation for data access while maintaining clean architecture principles.