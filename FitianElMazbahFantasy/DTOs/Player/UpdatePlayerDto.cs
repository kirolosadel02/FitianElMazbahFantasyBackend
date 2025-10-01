using System.ComponentModel.DataAnnotations;
using FitianElMazbahFantasy.Models;

namespace FitianElMazbahFantasy.DTOs.Player;

public class UpdatePlayerDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public PlayerPosition Position { get; set; }
    
    [Required]
    public int TeamId { get; set; }
}