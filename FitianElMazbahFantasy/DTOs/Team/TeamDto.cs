using System.ComponentModel.DataAnnotations;

namespace FitianElMazbahFantasy.DTOs.Team;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TeamWithPlayersDto : TeamDto
{
    public List<TeamPlayerDto> Players { get; set; } = new List<TeamPlayerDto>();
}

public class TeamPlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
}

public class CreateTeamDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    [Url(ErrorMessage = "LogoUrl must be a valid URL")]
    public string? LogoUrl { get; set; }
}

public class UpdateTeamDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    [Url(ErrorMessage = "LogoUrl must be a valid URL")]
    public string? LogoUrl { get; set; }
}