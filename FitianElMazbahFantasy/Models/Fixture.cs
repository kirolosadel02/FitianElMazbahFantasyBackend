namespace FitianElMazbahFantasy.Models;

public class Fixture
{
    public int Id { get; set; }
    public int Team1Id { get; set; }
    public int Team2Id { get; set; }
    public int MatchWeek { get; set; }
    public DateTime MatchDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Team Team1 { get; set; } = null!;
    public virtual Team Team2 { get; set; } = null!;
    public virtual MatchResult? MatchResult { get; set; }
    public virtual ICollection<MatchEvent> MatchEvents { get; set; } = new List<MatchEvent>();
}