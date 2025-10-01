using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Services.Interfaces;
using FitianElMazbahFantasy.DTOs.UserTeam;
using FitianElMazbahFantasy.Extensions;

namespace FitianElMazbahFantasy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserTeamsController : ControllerBase
{
    private readonly IUserTeamService _userTeamService;
    private readonly ILogger<UserTeamsController> _logger;

    public UserTeamsController(IUserTeamService userTeamService, ILogger<UserTeamsController> logger)
    {
        _userTeamService = userTeamService ?? throw new ArgumentNullException(nameof(userTeamService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Team Management

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserTeamDto>> GetUserTeam(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            var userTeam = await _userTeamService.GetUserTeamByIdAsync(id, cancellationToken);
            if (userTeam == null)
            {
                return NotFound();
            }

            // Users can only access their own team, admins can access any team
            if (currentUserRole != "Admin" && userTeam.UserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(userTeam.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user team with id {UserTeamId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/details")]
    [Authorize]
    public async Task<ActionResult<UserTeamDetailsDto>> GetUserTeamDetails(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            var userTeam = await _userTeamService.GetUserTeamWithDetailsAsync(id, cancellationToken);
            if (userTeam == null)
            {
                return NotFound();
            }

            // Users can only access their own team, admins can access any team
            if (currentUserRole != "Admin" && userTeam.UserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(userTeam.ToDetailsDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user team details with id {UserTeamId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<UserTeamDto>> GetUserTeamByUserId(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            // Users can only get their own team, admins can get any user's team
            if (currentUserRole != "Admin" && currentUserId != userId)
            {
                return Forbid();
            }

            var userTeam = await _userTeamService.GetUserTeamByUserIdAsync(userId, cancellationToken);
            if (userTeam == null)
            {
                return NotFound(new { message = "User does not have a team" });
            }

            return Ok(userTeam.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user team for user id {UserId}", userId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("user/{userId}/details")]
    [Authorize]
    public async Task<ActionResult<UserTeamDetailsDto>> GetUserTeamDetailsByUserId(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            // Users can only get their own team, admins can get any user's team
            if (currentUserRole != "Admin" && currentUserId != userId)
            {
                return Forbid();
            }

            var userTeam = await _userTeamService.GetUserTeamWithDetailsByUserIdAsync(userId, cancellationToken);
            if (userTeam == null)
            {
                return NotFound(new { message = "User does not have a team" });
            }

            return Ok(userTeam.ToDetailsDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user team details for user id {UserId}", userId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("my-team")]
    [Authorize]
    public async Task<ActionResult<UserTeamDto>> GetMyTeam(CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userTeam = await _userTeamService.GetUserTeamByUserIdAsync(currentUserId, cancellationToken);
            
            if (userTeam == null)
            {
                return NotFound(new { message = "You don't have a team yet" });
            }

            return Ok(userTeam.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user's team");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("my-team/details")]
    [Authorize]
    public async Task<ActionResult<UserTeamDetailsDto>> GetMyTeamDetails(CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userTeam = await _userTeamService.GetUserTeamWithDetailsByUserIdAsync(currentUserId, cancellationToken);
            
            if (userTeam == null)
            {
                return NotFound(new { message = "You don't have a team yet" });
            }

            return Ok(userTeam.ToDetailsDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user's team details");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<UserTeamDto>> CreateUserTeam(CreateUserTeamDto createTeamDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = GetCurrentUserId();

            // Check if user already has a team
            var hasTeam = await _userTeamService.UserHasTeamAsync(currentUserId, cancellationToken);
            if (hasTeam)
            {
                return BadRequest(new { message = "You already have a team. Each user can have only one team." });
            }

            var userTeam = new UserTeam
            {
                UserId = currentUserId,
                TeamName = createTeamDto.TeamName,
                TotalPoints = 0,
                IsLocked = false
            };

            var createdTeam = await _userTeamService.CreateUserTeamAsync(userTeam, cancellationToken);
            return CreatedAtAction(nameof(GetUserTeam), new { id = createdTeam.Id }, createdTeam.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user team");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<UserTeamDto>> UpdateUserTeam(int id, UpdateUserTeamDto updateTeamDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            var existingTeam = await _userTeamService.GetUserTeamByIdAsync(id, cancellationToken);
            if (existingTeam == null)
            {
                return NotFound();
            }

            // Users can only update their own team, admins can update any team
            if (currentUserRole != "Admin" && existingTeam.UserId != currentUserId)
            {
                return Forbid();
            }

            // Update allowed properties
            existingTeam.TeamName = updateTeamDto.TeamName;
            if (currentUserRole == "Admin")
            {
                existingTeam.TotalPoints = updateTeamDto.TotalPoints ?? existingTeam.TotalPoints;
                existingTeam.IsLocked = updateTeamDto.IsLocked ?? existingTeam.IsLocked;
            }

            var updatedTeam = await _userTeamService.UpdateUserTeamAsync(existingTeam, cancellationToken);
            return Ok(updatedTeam.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user team with id {UserTeamId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteUserTeam(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            var existingTeam = await _userTeamService.GetUserTeamByIdAsync(id, cancellationToken);
            if (existingTeam == null)
            {
                return NotFound();
            }

            // Users can only delete their own team, admins can delete any team
            if (currentUserRole != "Admin" && existingTeam.UserId != currentUserId)
            {
                return Forbid();
            }

            var result = await _userTeamService.DeleteUserTeamAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user team with id {UserTeamId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("check-has-team")]
    [Authorize]
    public async Task<ActionResult<UserHasTeamDto>> CheckUserHasTeam(CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userTeam = await _userTeamService.GetUserTeamByUserIdAsync(currentUserId, cancellationToken);
            
            return Ok(userTeam.ToHasTeamDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user has team");
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
}
