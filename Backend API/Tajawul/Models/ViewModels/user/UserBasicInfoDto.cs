using System;

namespace Tajawul.Models.ViewModels.user;

public class UserBasicInfoDto
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Username { get; set; }
    public string? ProfileImage { get; set; }
}
