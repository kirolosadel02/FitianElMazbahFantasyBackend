using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Services.Interfaces;
using FitianElMazbahFantasy.DTOs.Auth;

namespace FitianElMazbahFantasy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, IAuthService authService, ILogger<UsersController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Authentication Endpoints

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RefreshTokenAsync(refreshTokenDto, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default)
    {
        try
        {
            await _authService.LogoutAsync(refreshTokenDto.RefreshToken, cancellationToken);
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("logout-all")]
    [Authorize]
    public async Task<ActionResult> LogoutAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _authService.LogoutAllDevicesAsync(userId, cancellationToken);
            return Ok(new { message = "Logged out from all devices successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout all devices");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    #endregion

    #region User Management (Admin Only)

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt
            });
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<UserDto>> GetUser(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized access in GetUser for id {UserId}", id);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with id {UserId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUserProfile(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
            
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user profile");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with id {UserId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    #endregion

    #region Helper Methods

    private int GetCurrentUserId()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst("userId")?.Value;
            
            _logger.LogDebug("UserID claim value: {UserIdClaim}", userIdClaim);
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogError("User ID claim not found in token. Available claims: {Claims}", 
                    string.Join(", ", User.Claims.Select(c => $"{c.Type}:{c.Value}")));
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            
            if (!int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogError("Cannot parse User ID claim to integer. Value: {UserIdClaim}", userIdClaim);
                throw new UnauthorizedAccessException("Invalid User ID format in token");
            }
            
            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting user ID from claims");
            throw;
        }
    }

    private string GetCurrentUserRole()
    {
        try
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
            _logger.LogDebug("User role: {Role}", role);
            return role;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting user role from claims");
            return "User";
        }
    }

    #endregion

    [HttpGet("test-auth")]
    [Authorize]
    public ActionResult TestAuth()
    {
        try
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();
            
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated,
                UserId = userId,
                UserRole = userRole,
                Claims = claims
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test auth endpoint");
            return StatusCode(500, new { message = ex.Message });
        }
    }
}