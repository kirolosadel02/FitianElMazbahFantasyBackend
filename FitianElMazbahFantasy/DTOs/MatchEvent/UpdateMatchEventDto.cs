using System.ComponentModel.DataAnnotations;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.DTOs.MatchEvent;

public class UpdateMatchEventDto
{
    [Required]
    public int FixtureId { get; set; }
    
    [Required]
    public int PlayerId { get; set; }
    
    [Required]
    public EventType EventType { get; set; }
    
    [Required]
    public int Points { get; set; }
    
    [Range(1, 120)]
    public int? Minute { get; set; }
}