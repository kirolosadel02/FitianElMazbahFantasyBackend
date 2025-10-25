using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.DTOs.Auth;
using FitianElMazbahFantasy.Services.Interfaces;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.Configuration;

namespace FitianElMazbahFantasy.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Create new user
            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                Role = UserRole.User, // Always set to User role
                CreatedAt = DateTime.UtcNow
            };

            // Use Identity to create user with password
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = await _jwtService.CreateRefreshTokenAsync(user.Id, cancellationToken);

            _logger.LogInformation("User registered successfully: {Username}", user.UserName);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
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
            var user = await _userManager.FindByNameAsync(loginDto.UsernameOrEmail)
                ?? await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Use SignInManager to check password (handles lockout automatically)
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new UnauthorizedAccessException("Account locked due to multiple failed login attempts. Please try again later.");
                }
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = await _jwtService.CreateRefreshTokenAsync(user.Id, cancellationToken);

            _logger.LogInformation("User logged in successfully: {Username}", user.UserName);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
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

            _logger.LogInformation("Token refreshed for user: {Username}", refreshToken.User.UserName);

            return new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = new UserDto
                {
                    Id = refreshToken.User.Id,
                    Username = refreshToken.User.UserName,
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
}