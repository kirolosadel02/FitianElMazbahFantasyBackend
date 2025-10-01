using System.ComponentModel.DataAnnotations;

namespace FitianElMazbahFantasy.DTOs.Fixture;

public class FixtureDto
{
    public int Id { get; set; }
    public int Team1Id { get; set; }
    public string Team1Name { get; set; } = string.Empty;
    public int Team2Id { get; set; }
    public string Team2Name { get; set; } = string.Empty;
    public int MatchweekId { get; set; }
    public int MatchWeek { get; set; }
    public DateTime MatchDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}