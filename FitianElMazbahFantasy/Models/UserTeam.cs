namespace FitianElMazbahFantasy.Models;

public class UserTeam
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<UserTeamPlayer> UserTeamPlayers { get; set; } = new List<UserTeamPlayer>();
    public virtual ICollection<UserTeamMatchweekPoints> MatchweekPoints { get; set; } = new List<UserTeamMatchweekPoints>();
    public virtual ICollection<UserTeamSnapshot> TeamSnapshots { get; set; } = new List<UserTeamSnapshot>();
}