namespace FitianElMazbahFantasy.Models;

public class UserTeamPlayer
{
    public int Id { get; set; }
    public int UserTeamId { get; set; }
    public int PlayerId { get; set; }
    public DateTime AddedAt { get; set; }

    // Navigation properties
    public virtual UserTeam UserTeam { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;
}