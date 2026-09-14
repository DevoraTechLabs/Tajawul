using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.Filters
{
    public class EventUsersFilter
    {

        [StringLength(20, MinimumLength = 3, ErrorMessage = "relation must be between 3 and 20 characters")]
        public string? Relation { get; set; } = "attend";

    }
}
