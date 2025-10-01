using System.ComponentModel.DataAnnotations;

namespace FitianElMazbahFantasy.DTOs.Fixture;

public class CreateFixtureDto
{
    [Required]
    public int Team1Id { get; set; }
    
    [Required]
    public int Team2Id { get; set; }
    
    [Required]
    public int MatchweekId { get; set; }
    
    [Required]
    public DateTime MatchDate { get; set; }
}