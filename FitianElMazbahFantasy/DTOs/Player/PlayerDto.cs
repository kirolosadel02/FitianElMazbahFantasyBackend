using System.ComponentModel.DataAnnotations;

namespace FitianElMazbahFantasy.DTOs.Player;

public class PlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? TeamLogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PlayerWithStatsDto : PlayerDto
{
    public int TotalPoints { get; set; }
    public int GoalsScored { get; set; }
    public int Assists { get; set; }
    public int CleanSheets { get; set; }
    public double AveragePoints { get; set; }
}

public class CreatePlayerDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 4, ErrorMessage = "Position must be between 1 (Goalkeeper) and 4 (Forward)")]
    public int Position { get; set; } // PlayerPosition enum value
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "TeamId must be a positive number")]
    public int TeamId { get; set; }
}

public class UpdatePlayerDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 4, ErrorMessage = "Position must be between 1 (Goalkeeper) and 4 (Forward)")]
    public int Position { get; set; } // PlayerPosition enum value
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "TeamId must be a positive number")]
    public int TeamId { get; set; }
}

public class PlayerFilterDto
{
    [Range(1, 4, ErrorMessage = "Position must be between 1 (Goalkeeper) and 4 (Forward)")]
    public int? Position { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "TeamId must be a positive number")]
    public int? TeamId { get; set; }
    
    [StringLength(100)]
    public string? Name { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be positive")]
    public int PageNumber { get; set; } = 1;
    
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 20;
}