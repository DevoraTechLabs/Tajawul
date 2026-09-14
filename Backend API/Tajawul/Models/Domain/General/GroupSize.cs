namespace Tajawul.Models.Domain.General
{
    public class GroupSize
    {
        public string GroupId { get; set; } = null!;

        public string Group { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public DateTime AssignedDate { get; set; }
    }
}
