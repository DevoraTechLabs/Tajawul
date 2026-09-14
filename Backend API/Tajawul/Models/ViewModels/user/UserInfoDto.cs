using Tajawul.Models.Domain.Chatbot;

namespace Tajawul.Models.ViewModels.user
{
    public class UserInfoDto
    {
        public required string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> SpokenLanguages { get; set; } = new List<string>();
        public List<Message> Messages { get; set; } = new List<Message> { };
    }
}
