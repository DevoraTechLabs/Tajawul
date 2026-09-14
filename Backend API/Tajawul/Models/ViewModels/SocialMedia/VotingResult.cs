namespace Tajawul.Models.ViewModels.SocialMedia;


public class VotingResult
{
    public int? Upvotes { get; set; }
    public int? Downvotes { get; set; }
    public bool IsUpvotedByCurrentUser { get; set; }
    public bool IsDownvotedByCurrentUser { get; set; }
}



