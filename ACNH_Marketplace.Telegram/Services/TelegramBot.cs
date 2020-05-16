// <copyright file="TelegramBot.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
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
    public class TelegramBot : IDisposable
    {
        private readonly ILogger logger;
        private readonly BotConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBot"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        /// <param name="config"><see cref="BotConfiguration"/>.</param>
        public TelegramBot(ILogger<TelegramBot> logger, BotConfiguration config)
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

        /// <summary>
        /// Gets telegram client instance.
        /// </summary>
        public TelegramBotClient Client { get; private set; }

        /// <summary>
        /// Send text message to Telegram API.
        /// </summary>
        /// <param name="userId">Reciever user id.</param>
        /// <param name="message">Mesasge text.</param>
        /// <param name="replyTo">Reply to message id.</param>
        /// <param name="replyMarkup">Reply markup (custom keyboard).</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Message> SendMessageAsync(int userId, string message, int replyTo = 0, IReplyMarkup replyMarkup = null)
        {
            return await this.Client
                .SendTextMessageAsync(userId, message, ParseMode.Default, true, true, replyTo, replyMarkup);
        }

        /// <summary>
        /// Edit message by Telegram API.
        /// </summary>
        /// <param name="userId">Reciever user id.</param>
        /// <param name="messageId">Edit message id.</param>
        /// <param name="message">Mesasge text.</param>
        /// <param name="replyMarkup">Reply markup (custom keyboard).</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Message> EditMessage(
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