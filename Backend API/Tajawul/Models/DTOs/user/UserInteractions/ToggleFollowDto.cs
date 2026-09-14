using System;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.user.UserInteractions;

public class ToggleFollowDto
{
    [Required(ErrorMessage = "Followed Id is required")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "User not found")]
    public required string FollowedId { get; set; }
}