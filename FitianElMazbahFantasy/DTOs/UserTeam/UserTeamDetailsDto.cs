namespace FitianElMazbahFantasy.DTOs.UserTeam;

public class UserTeamDetailsDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<UserTeamPlayerDto> Players { get; set; } = new List<UserTeamPlayerDto>();
}

public class UserTeamPlayerDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}