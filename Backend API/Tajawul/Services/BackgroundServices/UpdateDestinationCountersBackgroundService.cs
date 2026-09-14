using Tajawul.Repositories;
using Tajawul.Repositories.User.Interaction;

namespace Tajawul.Services.BackgroundServices
{
    public class UpdateDestinationCountersBackgroundService: BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateDestinationCountersBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var interactionsRepository = scope.ServiceProvider.GetRequiredService<DestinationInteractionsRepository>();
                    await interactionsRepository.UpdateAllDestinationCountersAsync();
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
