using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.DTOs.Auth;

namespace FitianElMazbahFantasy.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAllDevicesAsync(int userId, CancellationToken cancellationToken = default);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}