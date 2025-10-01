using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FitianElMazbahFantasy.Data;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.Repositories.Implementations;
using FitianElMazbahFantasy.Services.Interfaces;
using FitianElMazbahFantasy.Services.Implementations;
using FitianElMazbahFantasy.Configuration;

namespace FitianElMazbahFantasy.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FitianElMazbahFantasyDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserTeamService, UserTeamService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JWT settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        
        // Get JWT settings for immediate use
        var jwtSettingsSection = configuration.GetSection(JwtSettings.SectionName);
        var secretKey = jwtSettingsSection["SecretKey"];
        var issuer = jwtSettingsSection["Issuer"];
        var audience = jwtSettingsSection["Audience"];
        
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not properly configured");
        }

        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Set to true in production
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Remove delay of token when expire
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
                    
                    if (!string.IsNullOrEmpty(authorizationHeader))
                    {
                        // If the header doesn't start with "Bearer ", automatically handle it
                        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            // Check if it looks like a JWT token (starts with 'eyJ' which is base64 encoded '{')
                            if (authorizationHeader.StartsWith("eyJ"))
                            {
                                context.Token = authorizationHeader;
                                var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                                logger?.LogDebug("Auto-handled JWT token without Bearer prefix");
                            }
                        }
                        else
                        {
                            // Extract token after "Bearer "
                            context.Token = authorizationHeader.Substring("Bearer ".Length);
                        }
                    }
                    
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                    logger?.LogError("Authentication failed: {Exception}", context.Exception);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                    logger?.LogDebug("Token validated for user: {User}", context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
        });

        return services;
    }
}