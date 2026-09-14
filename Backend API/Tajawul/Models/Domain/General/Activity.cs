namespace Tajawul.Models.Domain.General
{
    public class Activity
    {
        public string ActivityId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public DateTime AssignedDate { get; set; }
    }
}
