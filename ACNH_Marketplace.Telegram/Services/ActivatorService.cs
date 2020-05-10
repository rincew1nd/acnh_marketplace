using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ACNH_Marketplace.Telegram.Services
{
    public class ActivatorService : IHostedService
    {
        public ActivatorService(IBotService service, ILogger<ActivatorService> logger)
        {
            logger.LogInformation("Bot service activated");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
