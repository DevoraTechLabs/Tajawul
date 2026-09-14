using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Tajawul.Interfaces;
using Tajawul.Models.Domain;

namespace Tajawul.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<string> GetEmailTemplate(string templateName)
        {
            var filePath = Path.Combine("EmailTemplates", $"{templateName}.html");
            return await System.IO.File.ReadAllTextAsync(filePath);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            ;        // Create the email message
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message,
                TextBody = message
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            // Send the email using the SMTP server
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailSettings.Server, _emailSettings.Port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    // Handle the exception (log it or notify the admin)
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }
        }
    }
}
