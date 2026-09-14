namespace Tajawul.Models.Domain.Trips
{
    public class TripDuration
    {

        public required string Id { get; set; }
        public required string Name { get; set; }
        public string UserId { get; set; } = null!;

    }
}
