namespace Tajawul.Models.ViewModels.Review
{
    public class ReviewDto
    {
        public string ReviewId { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public required List<string> Creator { get; set; }

        public string DestinationId { get; set; } = null!;

        public string Comment { get; set; } = null!;

        public float Rate { get; set; }

        public DateTime Date { get; set; }
    }
}
