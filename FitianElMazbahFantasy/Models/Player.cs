namespace FitianElMazbahFantasy.Models;

public enum PlayerPosition
{
    Goalkeeper = 1,
    Defender = 2,
    Midfielder = 3,
    Forward = 4
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PlayerPosition Position { get; set; }
    public int TeamId { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Team Team { get; set; } = null!;
    public virtual ICollection<UserTeamPlayer> UserTeamPlayers { get; set; } = new List<UserTeamPlayer>();
    public virtual ICollection<MatchEvent> MatchEvents { get; set; } = new List<MatchEvent>();
}