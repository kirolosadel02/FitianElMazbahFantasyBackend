using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitianElMazbahFantasy.Models;
using FitianElMazbahFantasy.Services.Interfaces;
using FitianElMazbahFantasy.DTOs.Player;
using FitianElMazbahFantasy.Extensions;

namespace FitianElMazbahFantasy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly ILogger<PlayersController> _logger;

    public PlayersController(IPlayerService playerService, ILogger<PlayersController> logger)
    {
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers([FromQuery] PlayerFilterDto? filterDto = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Player> players;

            if (filterDto != null && (filterDto.Position.HasValue || filterDto.TeamId.HasValue || !string.IsNullOrEmpty(filterDto.Name)))
            {
                players = await _playerService.GetFilteredPlayersAsync(filterDto, cancellationToken);
            }
            else
            {
                players = await _playerService.GetAllPlayersAsync(cancellationToken);
            }

            var playerDtos = players.ToDto();
            return Ok(playerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<PlayerDto>> GetPlayer(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var player = await _playerService.GetPlayerByIdAsync(id, cancellationToken);
            if (player == null)
            {
                return NotFound();
            }

            return Ok(player.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player with id {PlayerId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("team/{teamId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayersByTeam(int teamId, CancellationToken cancellationToken = default)
    {
        try
        {
            var players = await _playerService.GetPlayersByTeamIdAsync(teamId, cancellationToken);
            var playerDtos = players.ToDto();
            return Ok(playerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players for team {TeamId}", teamId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("position/{position}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayersByPosition(int position, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Enum.IsDefined(typeof(PlayerPosition), position))
            {
                return BadRequest(new { message = "Invalid position value" });
            }

            var playerPosition = (PlayerPosition)position;
            var players = await _playerService.GetPlayersByPositionAsync(playerPosition, cancellationToken);
            var playerDtos = players.ToDto();
            return Ok(playerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players for position {Position}", position);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PlayerDto>> CreatePlayer(CreatePlayerDto createPlayerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.IsDefined(typeof(PlayerPosition), createPlayerDto.Position))
            {
                return BadRequest(new { message = "Invalid position value" });
            }

            var player = createPlayerDto.ToEntity();
            var createdPlayer = await _playerService.CreatePlayerAsync(player, cancellationToken);
            
            return CreatedAtAction(nameof(GetPlayer), new { id = createdPlayer.Id }, createdPlayer.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating player");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PlayerDto>> UpdatePlayer(int id, UpdatePlayerDto updatePlayerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.IsDefined(typeof(PlayerPosition), updatePlayerDto.Position))
            {
                return BadRequest(new { message = "Invalid position value" });
            }

            var existingPlayer = await _playerService.GetPlayerByIdAsync(id, cancellationToken);
            if (existingPlayer == null)
            {
                return NotFound();
            }

            updatePlayerDto.UpdateEntity(existingPlayer);
            var updatedPlayer = await _playerService.UpdatePlayerAsync(existingPlayer, cancellationToken);
            
            return Ok(updatedPlayer.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player with id {PlayerId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeletePlayer(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _playerService.DeletePlayerAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting player with id {PlayerId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("count")]
    [Authorize]
    public async Task<ActionResult<int>> GetPlayersCount(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _playerService.GetTotalPlayersCountAsync(cancellationToken);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players count");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}