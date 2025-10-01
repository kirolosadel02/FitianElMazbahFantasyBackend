using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Repositories.Interfaces;
using FitianElMazbahFantasy.DTOs.Team;

namespace FitianElMazbahFantasy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(IUnitOfWork unitOfWork, ILogger<TeamsController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams(CancellationToken cancellationToken = default)
    {
        try
        {
            var teams = await _unitOfWork.Repository<Team>().GetAllAsync(cancellationToken);
            var teamDtos = teams.Select(t => new TeamDto
            {
                Id = t.Id,
                Name = t.Name,
                LogoUrl = t.LogoUrl,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            });

            return Ok(teamDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting teams");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TeamWithPlayersDto>> GetTeam(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var team = await _unitOfWork.Repository<Team>().GetByIdAsync(id, cancellationToken, t => t.Players);
            if (team == null)
            {
                return NotFound();
            }

            var teamDto = new TeamWithPlayersDto
            {
                Id = team.Id,
                Name = team.Name,
                LogoUrl = team.LogoUrl,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt,
                Players = team.Players.Select(p => new TeamPlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position.ToString()
                }).ToList()
            };

            return Ok(teamDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team with id {TeamId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TeamDto>> CreateTeam(CreateTeamDto createTeamDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var team = new Team
            {
                Name = createTeamDto.Name,
                LogoUrl = createTeamDto.LogoUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Team>().AddAsync(team, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var teamDto = new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                LogoUrl = team.LogoUrl,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt
            };

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, teamDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TeamDto>> UpdateTeam(int id, UpdateTeamDto updateTeamDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTeam = await _unitOfWork.Repository<Team>().GetByIdAsync(id, cancellationToken);
            if (existingTeam == null)
            {
                return NotFound();
            }

            existingTeam.Name = updateTeamDto.Name;
            existingTeam.LogoUrl = updateTeamDto.LogoUrl;
            existingTeam.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Team>().Update(existingTeam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var teamDto = new TeamDto
            {
                Id = existingTeam.Id,
                Name = existingTeam.Name,
                LogoUrl = existingTeam.LogoUrl,
                CreatedAt = existingTeam.CreatedAt,
                UpdatedAt = existingTeam.UpdatedAt
            };

            return Ok(teamDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team with id {TeamId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTeam(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var team = await _unitOfWork.Repository<Team>().GetByIdAsync(id, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Check if team has players
            var hasPlayers = await _unitOfWork.Repository<Player>().AnyAsync(p => p.TeamId == id, cancellationToken);
            if (hasPlayers)
            {
                return BadRequest(new { message = "Cannot delete team with existing players" });
            }

            _unitOfWork.Repository<Team>().Remove(team);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team with id {TeamId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}