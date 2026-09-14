namespace Tajawul.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);

        Task<string> GetEmailTemplate(string templateName);

    }
}
