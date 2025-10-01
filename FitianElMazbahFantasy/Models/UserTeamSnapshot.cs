namespace FitianElMazbahFantasy.Models;

public class UserTeamSnapshot
{
    public int Id { get; set; }
    public int UserTeamId { get; set; }
    public int MatchweekId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime SnapshotDate { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual UserTeam UserTeam { get; set; } = null!;
    public virtual Matchweek Matchweek { get; set; } = null!;
    public virtual ICollection<UserTeamSnapshotPlayer> PlayerSnapshots { get; set; } = new List<UserTeamSnapshotPlayer>();
}