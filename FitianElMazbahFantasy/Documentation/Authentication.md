# JWT Authentication Implementation Details

## Configuration

### JWT Settings (appsettings.json)
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

### Service Registration (ServiceCollectionExtensions.cs)
```csharp
public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
    
    var jwtSettings = configuration.GetSection(JwtSettings.SectionName);
    var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    });

    return services;
}
```

## Token Structure

### Access Token Claims
- `NameIdentifier`: User ID (primary identifier)
- `Name`: Username
- `Email`: User email address
- `Role`: User role (Admin/User)
- Custom claims: `userId`, `username`

### Refresh Token
- Cryptographically secure random string
- Stored in database with expiration
- Individual and bulk revocation support

## Security Implementation

### Password Hashing
```csharp
public string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "SaltKey"));
    return Convert.ToBase64String(hashedBytes);
}
```

### Authorization Patterns
```csharp
// Controller level
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase { }

// Action level
[Authorize]
public async Task<ActionResult> GetProfile() { }

// Custom authorization logic
private string GetCurrentUserRole()
{
    return User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
}
```

## Admin User Creation

Admin users cannot be created via API registration. Create manually:

```sql
-- Create admin user in database
INSERT INTO Users (Username, Email, Password, Role, CreatedAt)
VALUES (
    'admin',
    'admin@fitianelmazbah.com',
    'YourHashedPasswordHere', -- Use application's hash method
    2, -- UserRole.Admin = 2
    GETUTCDATE()
);
```

## Token Management

### Refresh Token Flow
1. User logs in ? receives access token + refresh token
2. Access token expires ? use refresh token to get new access token
3. Refresh token expires ? user must login again

### Token Revocation
- **Single device logout**: Revokes specific refresh token
- **All devices logout**: Revokes all user's refresh tokens
- **Admin revocation**: Admin can revoke any user's tokens

## Production Security Considerations

1. **HTTPS Only**: Set `RequireHttpsMetadata = true`
2. **Secure Storage**: Store refresh tokens in HTTP-only cookies
3. **Rate Limiting**: Implement rate limiting for auth endpoints
4. **Enhanced Hashing**: Use BCrypt.Net instead of SHA256
5. **Audit Logging**: Log all authentication events
6. **Token Blacklisting**: Consider token blacklisting for immediate revocation

## Middleware Pipeline Order

```csharp
app.UseAuthentication();  // Validates JWT tokens
app.UseAuthorization();   // Enforces role-based policies
app.MapControllers();     // Routes to controllers
```

This implementation provides secure JWT-based authentication with role-based authorization suitable for the fantasy football system.