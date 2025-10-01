using System.ComponentModel.DataAnnotations;

namespace FitianElMazbahFantasy.DTOs.UserTeam
{
    public class CreateUserTeamDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string TeamName { get; set; } = string.Empty;
    }
}
