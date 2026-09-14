namespace Tajawul.Models.Domain.General
{
    public class Types
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime AssignedDate { get; set; }

    }
}
