namespace FitianElMazbahFantasy.Models;

public class UserTeamSnapshotPlayer
{
    public int Id { get; set; }
    public int SnapshotId { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public PlayerPosition Position { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }

    // Navigation properties
    public virtual UserTeamSnapshot Snapshot { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;
}