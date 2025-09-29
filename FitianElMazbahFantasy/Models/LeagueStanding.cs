namespace FitianElMazbahFantasy.Models;

public class LeagueStanding
{
    public int Id { get; set; }
    public int UserTeamId { get; set; }
    public int Position { get; set; }
    public int TotalPoints { get; set; }
    public int MatchWeek { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual UserTeam UserTeam { get; set; } = null!;
}