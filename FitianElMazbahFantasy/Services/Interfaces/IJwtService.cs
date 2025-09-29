using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Configuration;

namespace FitianElMazbahFantasy.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task<RefreshToken> CreateRefreshTokenAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAllUserRefreshTokensAsync(int userId, CancellationToken cancellationToken = default);
}