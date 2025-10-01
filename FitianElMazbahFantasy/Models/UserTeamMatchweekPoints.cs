namespace FitianElMazbahFantasy.Models;

public class UserTeamMatchweekPoints
{
    public int UserTeamId { get; set; }
    public int MatchweekId { get; set; }
    public int Points { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int CleanSheets { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int Saves { get; set; }
    public int Penalties { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual UserTeam UserTeam { get; set; } = null!;
    public virtual Matchweek Matchweek { get; set; } = null!;
}