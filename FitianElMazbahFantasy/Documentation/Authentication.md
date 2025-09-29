# JWT Authentication & Authorization Implementation

## Overview

This implementation provides a comprehensive JWT-based authentication and authorization system for the FitianElMazbahFantasy application with role-based access control.

## Key Features

### ?? **Security Features**
- JWT Access Tokens with configurable expiration
- Refresh Tokens for secure token renewal
- Role-based authorization (Admin/User)
- Password hashing with salt
- Token revocation support
- Admin role protection (cannot be set via API)

### ?? **Authentication Endpoints**

#### Public Endpoints (No Authentication Required)
```
POST /api/users/register     - User registration (User role only)
POST /api/users/login        - User login
POST /api/users/refresh-token - Token refresh
```

#### Protected Endpoints (Authentication Required)
```
POST /api/users/logout       - Logout (revoke refresh token)
POST /api/users/logout-all   - Logout from all devices
GET  /api/users/profile      - Get current user profile
```

#### Admin Only Endpoints
```
GET    /api/users            - Get all users
DELETE /api/users/{id}       - Delete user
```

#### User/Admin Endpoints (Self or Admin Access)
```
GET /api/users/{id}          - Get user by ID
GET /api/users/{id}/teams    - Get user with teams
```

## Architecture

### 1. JWT Configuration
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

### 2. Token Structure
**Access Token Claims:**
- `NameIdentifier`: User ID
- `Name`: Username
- `Email`: User Email
- `Role`: User Role (Admin/User)
- Custom claims: `userId`, `username`

**Refresh Token:**
- Cryptographically secure random string
- Stored in database with expiration
- Can be revoked individually or all at once

### 3. Role-Based Authorization

#### User Role (Default)
- Can register via API
- Access to own profile and teams
- Can create fantasy teams
- Can participate in leagues

#### Admin Role (Manual Only)
- Cannot be set via registration API
- Must be created manually in database
- Full access to user management
- Can manage teams, players, fixtures
- Can view all user data

## Security Implementation

### Password Security
```csharp
// Simple hashing implementation (enhance for production)
public string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "SaltKey"));
    return Convert.ToBase64String(hashedBytes);
}
```

### Token Validation
- Signature validation using HMAC SHA256
- Issuer and Audience validation
- Expiration time validation
- Zero clock skew for precise timing

### Authorization Policies
```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
});
```

## Usage Examples

### 1. User Registration
```http
POST /api/users/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "securePassword123",
  "confirmPassword": "securePassword123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64EncodedRefreshToken",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "role": "User",
    "createdAt": "2024-01-01T11:00:00Z"
  }
}
```

### 2. User Login
```http
POST /api/users/login
Content-Type: application/json

{
  "usernameOrEmail": "johndoe",
  "password": "securePassword123"
}
```

### 3. Token Refresh
```http
POST /api/users/refresh-token
Content-Type: application/json

{
  "refreshToken": "base64EncodedRefreshToken"
}
```

### 4. Accessing Protected Endpoints
```http
GET /api/users/profile
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Admin User Creation

Since admin users cannot be created via the API, they must be added manually:

### SQL Script to Create Admin User
```sql
-- First, create the admin user
INSERT INTO Users (Username, Email, Password, Role, CreatedAt)
VALUES (
    'admin',
    'admin@fitianelmazbah.com',
    'HashedPasswordHere', -- Use the same hashing method as the application
    2, -- UserRole.Admin = 2
    GETUTCDATE()
);
```

### Using the Application's Hash Method
```csharp
// In a console app or startup code
var authService = serviceProvider.GetService<IAuthService>();
var hashedPassword = authService.HashPassword("AdminPassword123");
// Use this hashedPassword in the SQL insert
```

## Middleware Pipeline

The authentication middleware is configured in the correct order:

1. `UseAuthentication()` - Validates JWT tokens
2. `UseAuthorization()` - Enforces role-based policies
3. `MapControllers()` - Routes to controllers with `[Authorize]` attributes

## Swagger Integration

JWT authentication is integrated with Swagger UI:
- Authorization button in Swagger UI
- Enter token as: `Bearer your-jwt-token-here`
- Test protected endpoints directly from Swagger

## Best Practices Implemented

### 1. **Security**
- Tokens stored in memory (client-side)
- Refresh tokens in secure HTTP-only cookies (recommended for production)
- Admin role cannot be assigned via API
- Comprehensive input validation

### 2. **Error Handling**
- Structured error responses
- Detailed logging without exposing sensitive data
- Proper HTTP status codes

### 3. **Token Management**
- Short-lived access tokens (60 minutes)
- Long-lived refresh tokens (7 days)
- Token revocation on logout
- Ability to logout from all devices

### 4. **Authorization Patterns**
- Method-level authorization with `[Authorize]` attributes
- Role-based policies
- Resource-based authorization (users can only access their own data)

## Production Considerations

1. **Use HTTPS Only**: Set `RequireHttpsMetadata = true`
2. **Secure Refresh Tokens**: Store in HTTP-only cookies
3. **Enhanced Password Hashing**: Use BCrypt.Net or ASP.NET Core Identity
4. **Rate Limiting**: Implement rate limiting for auth endpoints
5. **Audit Logging**: Log all authentication events
6. **Token Blacklisting**: Consider implementing token blacklisting for immediate revocation

This implementation provides a solid foundation for authentication and authorization while maintaining security best practices and flexibility for future enhancements.