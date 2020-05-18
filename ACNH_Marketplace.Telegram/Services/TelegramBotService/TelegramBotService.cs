// <copyright file="TelegramBotService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services.BotService
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using global::Telegram.Bot;
    using global::Telegram.Bot.Types;
    using global::Telegram.Bot.Types.Enums;
    using global::Telegram.Bot.Types.ReplyMarkups;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Telegram Bot client.
    /// </summary>
    public class TelegramBotService : ITelegramBotService, IDisposable
    {
        private readonly ILogger logger;
        private readonly BotConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotService"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        /// <param name="config"><see cref="BotConfiguration"/>.</param>
        public TelegramBotService(ILogger<TelegramBotDevService> logger, BotConfiguration config)
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
        public async Task<Message> SendMessageAsync(int userId, string message, int replyTo = 0, IReplyMarkup replyMarkup = null)
        {
            return await this.Client
                .SendTextMessageAsync(userId, message, ParseMode.Default, true, true, replyTo, replyMarkup);
        }

        /// <inheritdoc/>
        public async Task<Message> EditMessageAsync(
            int userId, int? messageId, string message = null, InlineKeyboardMarkup replyMarkup = null)
        {
            if (!messageId.HasValue)
            {
                return await this.SendMessageAsync(userId, message, 0, replyMarkup);
            }
            else if (message == null)
            {
                return await this.Client
                    .EditMessageReplyMarkupAsync(userId, messageId.Value, replyMarkup);
            }
            else
            {
                return await this.Client
                    .EditMessageTextAsync(userId, messageId.Value, message, ParseMode.Default, true, replyMarkup);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Client.DeleteWebhookAsync().Wait();
            this.Client = null;
        }
    }
}