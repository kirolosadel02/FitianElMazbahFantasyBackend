using Microsoft.AspNetCore.Identity;

namespace FitianElMazbahFantasy.Models;

public enum UserRole
{
    User = 1,
    Admin = 2
}

public class User : IdentityUser<int>
{
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual UserTeam? UserTeam { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}