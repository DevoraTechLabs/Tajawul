using Tajawul.Repositories;

namespace Tajawul.Services.Reviews
{
    public class RatingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RatingBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var reviewRepository = scope.ServiceProvider.GetRequiredService<ReviewRepository>();
                    await reviewRepository.UpdateAverageRatingsAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
