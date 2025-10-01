namespace FitianElMazbahFantasy.DTOs.UserTeam;

public class UserTeamDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int PlayerCount { get; set; }
}