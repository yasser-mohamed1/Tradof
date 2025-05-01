using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tradof.Project.Services.Interfaces;

namespace Tradof.Project.Services.BackgroundServices
{
    public class ProjectNotificationBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ProjectNotificationBackgroundService> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<ProjectNotificationBackgroundService> _logger = logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<IProjectNotificationService>();
                        await notificationService.SendProjectEndDateNotificationsAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while sending project end date notifications.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}