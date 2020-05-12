using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Telegram.Bot;

namespace ACNH_Marketplace.Telegram.Services
{
    public class BotService : IBotService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly BotConfiguration _config;

        public TelegramBotClient Client { get; private set; }

        public BotService(ILogger<BotService> logger, BotConfiguration config)
        {
            _logger = logger;
            _config = config;

            if (config.Proxy != null)
            {
                Client = new TelegramBotClient(
                    config.Token,
                    new WebProxy(config.Proxy.Address, config.Proxy.Port)
                    {
                        Credentials = new NetworkCredential(config.Proxy.User, config.Proxy.Password)
                    }
                );
            }
            else
            {
                Client = new TelegramBotClient(config.Token);
            }

            Client.SetWebhookAsync(_config.WebhookURL).Wait();

            var me = Client.GetMeAsync().Result;
            _logger.LogInformation($"Bot {me.Id}-{me.FirstName} is active.");
        }

        public void Dispose()
        {
            Client.DeleteWebhookAsync().Wait();
            Client = null;
        }
    }
}