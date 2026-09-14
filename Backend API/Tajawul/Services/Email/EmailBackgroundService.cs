using Tajawul.Interfaces;
using System.Threading.Channels;

namespace Tajawul.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IEmailService _emailService;
        private readonly Channel<(string Email, string Subject, string Message)> _channel;

        public EmailBackgroundService(IEmailService emailService)
        {
            _emailService = emailService;
            _channel = Channel.CreateUnbounded<(string Email, string Subject, string Message)>();
        }

        public async Task QueueEmailAsync(string email, string subject, string message)
        {
            await _channel.Writer.WriteAsync((email, subject, message));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var emailRequest = await _channel.Reader.ReadAsync(stoppingToken);

                // Send email
                await _emailService.SendEmailAsync(emailRequest.Email, emailRequest.Subject, emailRequest.Message);
            }
        }
    }
}
