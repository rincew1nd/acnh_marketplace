using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ACNH_Marketplace.Telegram
{
    public class TelegramBotService : IHostedService
    {
        private MarketplaceBot _bot;
        private ILogger _logger;

        public TelegramBotService(ILogger<TelegramBotService> logger, MarketplaceBot bot)
        {
            _logger = logger;
            _bot = bot;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ACNH Marketplace service is started.");

            _bot.StartReceiving(cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ACNH Marketplace service is stopping.");

            _bot.StopReceiving();

            return Task.CompletedTask;
        }
    }
}
