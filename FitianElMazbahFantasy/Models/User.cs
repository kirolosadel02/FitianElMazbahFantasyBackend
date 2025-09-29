namespace FitianElMazbahFantasy.Models;

public enum UserRole
{
    User = 1,
    Admin = 2
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}