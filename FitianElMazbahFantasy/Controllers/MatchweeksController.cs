using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Services.Interfaces;
using FitianElMazbahFantasy.DTOs.Matchweek;

namespace FitianElMazbahFantasy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchweeksController : ControllerBase
{
    private readonly IMatchweekService _matchweekService;
    private readonly ILogger<MatchweeksController> _logger;

    public MatchweeksController(IMatchweekService matchweekService, ILogger<MatchweeksController> logger)
    {
        _matchweekService = matchweekService ?? throw new ArgumentNullException(nameof(matchweekService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MatchweekDto>>> GetMatchweeks(CancellationToken cancellationToken = default)
    {
        try
        {
            var matchweeks = await _matchweekService.GetAllMatchweeksAsync(cancellationToken);
            var matchweekDtos = matchweeks.Select(m => new MatchweekDto
            {
                Id = m.Id,
                WeekNumber = m.WeekNumber,
                DeadlineDate = m.DeadlineDate,
                IsActive = m.IsActive,
                IsCompleted = m.IsCompleted,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).OrderBy(m => m.WeekNumber);

            return Ok(matchweekDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matchweeks");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<ActionResult<MatchweekDto>> GetCurrentMatchweek(CancellationToken cancellationToken = default)
    {
        try
        {
            var matchweek = await _matchweekService.GetCurrentMatchweekAsync(cancellationToken);
            if (matchweek == null)
            {
                return NotFound(new { message = "No current matchweek found" });
            }

            var matchweekDto = new MatchweekDto
            {
                Id = matchweek.Id,
                WeekNumber = matchweek.WeekNumber,
                DeadlineDate = matchweek.DeadlineDate,
                IsActive = matchweek.IsActive,
                IsCompleted = matchweek.IsCompleted,
                CreatedAt = matchweek.CreatedAt,
                UpdatedAt = matchweek.UpdatedAt
            };

            return Ok(matchweekDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current matchweek");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<MatchweekDto>> GetMatchweek(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var matchweek = await _matchweekService.GetMatchweekByIdAsync(id, cancellationToken);
            if (matchweek == null)
            {
                return NotFound();
            }

            var matchweekDto = new MatchweekDto
            {
                Id = matchweek.Id,
                WeekNumber = matchweek.WeekNumber,
                DeadlineDate = matchweek.DeadlineDate,
                IsActive = matchweek.IsActive,
                IsCompleted = matchweek.IsCompleted,
                CreatedAt = matchweek.CreatedAt,
                UpdatedAt = matchweek.UpdatedAt
            };

            return Ok(matchweekDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting matchweek with id {MatchweekId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MatchweekDto>> CreateMatchweek(CreateMatchweekDto createMatchweekDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var matchweek = new Matchweek
            {
                WeekNumber = createMatchweekDto.WeekNumber,
                DeadlineDate = createMatchweekDto.DeadlineDate,
                IsActive = createMatchweekDto.IsActive,
                IsCompleted = false
            };

            var createdMatchweek = await _matchweekService.CreateMatchweekAsync(matchweek, cancellationToken);
            
            var matchweekDto = new MatchweekDto
            {
                Id = createdMatchweek.Id,
                WeekNumber = createdMatchweek.WeekNumber,
                DeadlineDate = createdMatchweek.DeadlineDate,
                IsActive = createdMatchweek.IsActive,
                IsCompleted = createdMatchweek.IsCompleted,
                CreatedAt = createdMatchweek.CreatedAt,
                UpdatedAt = createdMatchweek.UpdatedAt
            };

            return CreatedAtAction(nameof(GetMatchweek), new { id = createdMatchweek.Id }, matchweekDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating matchweek");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MatchweekDto>> UpdateMatchweek(int id, UpdateMatchweekDto updateMatchweekDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingMatchweek = await _matchweekService.GetMatchweekByIdAsync(id, cancellationToken);
            if (existingMatchweek == null)
            {
                return NotFound();
            }

            existingMatchweek.WeekNumber = updateMatchweekDto.WeekNumber;
            existingMatchweek.DeadlineDate = updateMatchweekDto.DeadlineDate;
            existingMatchweek.IsActive = updateMatchweekDto.IsActive;
            existingMatchweek.IsCompleted = updateMatchweekDto.IsCompleted;

            var updatedMatchweek = await _matchweekService.UpdateMatchweekAsync(existingMatchweek, cancellationToken);
            
            var matchweekDto = new MatchweekDto
            {
                Id = updatedMatchweek.Id,
                WeekNumber = updatedMatchweek.WeekNumber,
                DeadlineDate = updatedMatchweek.DeadlineDate,
                IsActive = updatedMatchweek.IsActive,
                IsCompleted = updatedMatchweek.IsCompleted,
                CreatedAt = updatedMatchweek.CreatedAt,
                UpdatedAt = updatedMatchweek.UpdatedAt
            };

            return Ok(matchweekDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating matchweek with id {MatchweekId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteMatchweek(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _matchweekService.DeleteMatchweekAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting matchweek with id {MatchweekId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}