namespace FitianElMazbahFantasy.Models;

public enum EventType
{
    Goal = 1,
    Assist = 2,
    CleanSheet = 3,
    YellowCard = 4,
    RedCard = 5,
    Save = 6,
    Penalty = 7
}

public class MatchEvent
{
    public int Id { get; set; }
    public int FixtureId { get; set; }
    public int PlayerId { get; set; }
    public EventType EventType { get; set; }
    public int Points { get; set; }
    public int? Minute { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Fixture Fixture { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;
}