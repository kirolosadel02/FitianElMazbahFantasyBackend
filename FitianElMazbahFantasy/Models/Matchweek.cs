namespace FitianElMazbahFantasy.Models;

public class Matchweek
{
    public int Id { get; set; }
    public int WeekNumber { get; set; }
    public DateTime DeadlineDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Fixture> Fixtures { get; set; } = new List<Fixture>();
    public virtual ICollection<UserTeamMatchweekPoints> UserTeamMatchweekPoints { get; set; } = new List<UserTeamMatchweekPoints>();
    public virtual ICollection<UserTeamSnapshot> UserTeamSnapshots { get; set; } = new List<UserTeamSnapshot>();
}