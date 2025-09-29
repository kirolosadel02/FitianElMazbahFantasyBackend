# Repository Pattern & Unit of Work Implementation

## Overview

This implementation follows best practices for the Repository Pattern and Unit of Work in .NET 8, providing a clean abstraction layer over Entity Framework Core.

## Architecture

### 1. Generic Repository Pattern
- **Interface**: `IGenericRepository<T>`
- **Implementation**: `GenericRepository<T>`
- **Benefits**:
  - Code reusability across all entities
  - Consistent API for CRUD operations
  - Support for includes/eager loading
  - Pagination support
  - CancellationToken support for async operations

### 2. Unit of Work Pattern
- **Interface**: `IUnitOfWork`
- **Implementation**: `UnitOfWork`
- **Features**:
  - Transaction management
  - Single point of save changes
  - Repository factory pattern
  - Thread-safe repository caching
  - Proper disposal pattern

### 3. Service Layer
- **Example**: `IUserService` / `UserService`
- **Purpose**: 
  - Business logic encapsulation
  - Transaction coordination
  - Error handling and logging
  - Validation

## Key Features

### Generic Repository Features
```csharp
// Basic operations
Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

// With includes (eager loading)
Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);

// Querying
Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

// Pagination
Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

// CRUD operations
Task AddAsync(T entity, CancellationToken cancellationToken = default);
void Update(T entity);
void Remove(T entity);
```

### Unit of Work Features
```csharp
// Repository factory
IGenericRepository<T> Repository<T>() where T : class;

// Transaction management
Task BeginTransactionAsync(CancellationToken cancellationToken = default);
Task CommitTransactionAsync(CancellationToken cancellationToken = default);
Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

// Save changes
Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
```

## Usage Examples

### Basic Repository Usage
```csharp
// Dependency injection
public class UserService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // Get user with teams
    public async Task<User?> GetUserWithTeamsAsync(int id)
    {
        return await _unitOfWork.Repository<User>()
            .GetByIdAsync(id, u => u.UserTeams);
    }
    
    // Create user with validation
    public async Task<User> CreateUserAsync(User user)
    {
        await _unitOfWork.Repository<User>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user;
    }
}
```

### Transaction Usage
```csharp
public async Task<UserTeam> CreateUserTeamWithPlayersAsync(UserTeam team, List<Player> players)
{
    await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        // Add team
        await _unitOfWork.Repository<UserTeam>().AddAsync(team);
        await _unitOfWork.SaveChangesAsync();
        
        // Add players
        foreach(var player in players)
        {
            var userTeamPlayer = new UserTeamPlayer 
            { 
                UserTeamId = team.Id, 
                PlayerId = player.Id 
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

### 1. **Testability**
- Easy to mock repositories for unit testing
- Clear separation of concerns
- Dependency injection friendly

### 2. **Performance**
- ConcurrentDictionary for repository caching
- Proper async/await patterns
- CancellationToken support
- Efficient pagination

### 3. **Maintainability**
- Single responsibility principle
- Consistent API across all entities
- Centralized transaction management
- Proper error handling

### 4. **Flexibility**
- Generic implementation reduces code duplication
- Easy to extend with specific repositories if needed
- Support for complex queries with includes

## Best Practices Implemented

1. **Async/Await**: All operations are async with CancellationToken support
2. **Disposal Pattern**: Proper resource cleanup in UnitOfWork
3. **Thread Safety**: ConcurrentDictionary for repository caching
4. **Error Handling**: Comprehensive exception handling in services
5. **Logging**: Structured logging throughout the application
6. **Validation**: Business rule validation in service layer
7. **Transaction Management**: Explicit transaction control when needed

## Registration in DI Container

```csharp
// In ServiceCollectionExtensions
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IUserService, UserService>();
```

This implementation provides a robust foundation for data access in your fantasy football application while maintaining clean architecture principles and best practices.