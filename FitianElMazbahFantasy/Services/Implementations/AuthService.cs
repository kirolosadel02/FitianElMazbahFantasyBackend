using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.DTOs.Auth;
using FitianElMazbahFantasy.Services.Interfaces;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.Configuration;

namespace FitianElMazbahFantasy.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if username exists
            var existingUserByUsername = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username, cancellationToken);
            
            if (existingUserByUsername != null)
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Check if email exists
            var existingUserByEmail = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email, cancellationToken);
                
            if (existingUserByEmail != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Create new user - Only allow User role registration
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                Password = HashPassword(registerDto.Password),
                Role = UserRole.User, // Always set to User role
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = await _jwtService.CreateRefreshTokenAsync(user.Id, cancellationToken);

            _logger.LogInformation("User registered successfully: {Username}", user.Username);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            throw;
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find user by username or email
            var user = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Username == loginDto.UsernameOrEmail || u.Email == loginDto.UsernameOrEmail, cancellationToken);

            if (user == null || !VerifyPassword(loginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = await _jwtService.CreateRefreshTokenAsync(user.Id, cancellationToken);

            _logger.LogInformation("User logged in successfully: {Username}", user.Username);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            throw;
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate refresh token
            var isValidRefreshToken = await _jwtService.ValidateRefreshTokenAsync(refreshTokenDto.RefreshToken, cancellationToken);
            if (!isValidRefreshToken)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Get refresh token from database
            var refreshToken = await _unitOfWork.Repository<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken, cancellationToken, rt => rt.User);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Revoke the old refresh token
            await _jwtService.RevokeRefreshTokenAsync(refreshTokenDto.RefreshToken, cancellationToken);

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.User);
            var newRefreshToken = await _jwtService.CreateRefreshTokenAsync(refreshToken.UserId, cancellationToken);

            _logger.LogInformation("Token refreshed for user: {Username}", refreshToken.User.Username);

            return new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = new UserDto
                {
                    Id = refreshToken.User.Id,
                    Username = refreshToken.User.Username,
                    Email = refreshToken.User.Email,
                    Role = refreshToken.User.Role.ToString(),
                    CreatedAt = refreshToken.User.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw;
        }
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jwtService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
            _logger.LogInformation("User logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            throw;
        }
    }

    public async Task LogoutAllDevicesAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _jwtService.RevokeAllUserRefreshTokensAsync(userId, cancellationToken);
            _logger.LogInformation("User logged out from all devices: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout all devices");
            throw;
        }
    }

    public string HashPassword(string password)
    {
        // Using BCrypt for password hashing (simple implementation for demo)
        // In production, consider using ASP.NET Core Identity or BCrypt.Net
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "SaltKey"));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hashToVerify = HashPassword(password);
        return hashToVerify == hashedPassword;
    }
}