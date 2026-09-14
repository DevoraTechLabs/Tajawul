using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tajawul.Models.Domain.Users;

public partial class Person : IdentityUser
{
    //public int PersonId { get; set; }

    //public string Email { get; set; } = null!;

    //public string Password { get; set; } = null!;

    //[NotMapped] // This prevents EF from creating a column for UserName
    //public override string UserName { get; set; }

    //public string FirstName { get; set; } = null!;

    //public string LastName { get; set; } = null!;

    //public string? ProfilePictureUrl { get; set; }

    //public string? PhoneNumber { get; set; }

    //public DateOnly? BirthDate { get; set; }

    //public bool EmailConfirmed { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    //public string? PostalCode { get; set; }

    //public string? City { get; set; }

    //public string? Country { get; set; }

    //public string? Street { get; set; }

    //public string? Location { get; set; }

    public string? RefreshToken { get; set; }

    //public virtual ICollection<BusinessManager> BusinessManagers { get; set; } = new List<BusinessManager>();

    public virtual ICollection<PlaceOwner> PlaceOwners { get; set; } = new List<PlaceOwner>();

    //public virtual ICollection<User> Users { get; set; } = new List<User>();



}
