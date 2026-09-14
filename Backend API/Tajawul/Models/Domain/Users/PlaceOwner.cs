namespace Tajawul.Models.Domain.Users;

public partial class PlaceOwner
{
    public string OwnerId { get; set; } = null!;
    public string PersonId { get; set; } = null!;

    public string NationalNumber { get; set; } = null!;

    public string? DocumentImageUrl { get; set; }

    public virtual Person Person { get; set; } = null!;
}
