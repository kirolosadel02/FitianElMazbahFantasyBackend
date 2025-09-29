namespace FitianElMazbahFantasy.Models;

public class MatchResult
{
    public int Id { get; set; }
    public int FixtureId { get; set; }
    public string FinalScore { get; set; } = string.Empty; // e.g., "2-1", "0-0"
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Fixture Fixture { get; set; } = null!;
}