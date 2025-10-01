using System.ComponentModel.DataAnnotations;

namespace FitianElMazbahFantasy.DTOs.UserTeam
{
    public class UpdateUserTeamDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string TeamName { get; set; } = string.Empty;

        // Only admins can update these fields
        public int? TotalPoints { get; set; }
        public bool? IsLocked { get; set; }
    }
}
