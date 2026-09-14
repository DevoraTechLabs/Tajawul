namespace Tajawul.Models.ViewModels.user.Chatbot
{
    public class MessageDto
    {
        public required string Prompt {  get; set; }
        public required string Response { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
