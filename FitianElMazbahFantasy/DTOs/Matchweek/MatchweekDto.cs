using System.ComponentModel.DataAnnotations;

namespace FitianElMazbahFantasy.DTOs.Matchweek;

public class MatchweekDto
{
    public int Id { get; set; }
    public int WeekNumber { get; set; }
    public DateTime DeadlineDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateMatchweekDto
{
    [Required]
    [Range(1, 38, ErrorMessage = "Week number must be between 1 and 38")]
    public int WeekNumber { get; set; }
    
    [Required]
    public DateTime DeadlineDate { get; set; }
    
    public bool IsActive { get; set; } = false;
}

public class UpdateMatchweekDto
{
    [Required]
    [Range(1, 38, ErrorMessage = "Week number must be between 1 and 38")]
    public int WeekNumber { get; set; }
    
    [Required]
    public DateTime DeadlineDate { get; set; }
    
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
}