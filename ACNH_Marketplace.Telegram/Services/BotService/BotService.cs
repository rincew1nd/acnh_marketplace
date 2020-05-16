// <copyright file="BotService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using System;
    using System.Net;
    using global::Telegram.Bot;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc/>
    public class BotService : IBotService, IDisposable
    {
        private readonly ILogger logger;
        private readonly BotConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotService"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        /// <param name="config"><see cref="BotConfiguration"/>.</param>
        public BotService(ILogger<BotService> logger, BotConfiguration config)
        {
            this.logger = logger;
            this.config = config;

            if (config.Proxy != null)
            {
                this.Client = new TelegramBotClient(
                    config.Token,
                    new WebProxy(config.Proxy.Address, config.Proxy.Port)
                    {
                        Credentials = new NetworkCredential(config.Proxy.Login, config.Proxy.Password),
                    });
            }
            else
            {
                this.Client = new TelegramBotClient(config.Token);
            }

            this.Client.SetWebhookAsync(this.config.WebhookUri).Wait();

            var me = this.Client.GetMeAsync().Result;
            this.logger.LogInformation($"Bot {me.Id}-{me.FirstName} is active.");
        }

        /// <inheritdoc/>
        public TelegramBotClient Client { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Client.DeleteWebhookAsync().Wait();
            this.Client = null;
        }
    }
}