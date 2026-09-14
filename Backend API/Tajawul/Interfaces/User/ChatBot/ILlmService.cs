namespace Tajawul.Interfaces.User.ChatBot
{
    public interface ILlmService
    {
        Task<string?> GetLlmResponseAsync(string prompt, string token);
    }
}
