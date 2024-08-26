using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class SessionStatusUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SessionStatusUpdaterService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1);

        public SessionStatusUpdaterService(IServiceProvider serviceProvider, ILogger<SessionStatusUpdaterService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Session Status Updater Service is starting.");

            var now = DateTime.UtcNow.AddHours(7);
            var nextRunTime = now.Date.AddHours(5).AddMinutes(50); 
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            var initialDelay = nextRunTime - now;

            await Task.Delay(initialDelay, stoppingToken); 

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                        await sessionService.UpdateSessionStatus();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating session status.");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); 
            }

            _logger.LogInformation("Session Status Updater Service is stopping.");
        }
    }

}
