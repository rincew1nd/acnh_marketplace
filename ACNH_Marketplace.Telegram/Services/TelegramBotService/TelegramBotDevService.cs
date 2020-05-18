// <copyright file="TelegramBotDevService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services.BotService
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.Telegram.Helpers;
    using global::Telegram.Bot;
    using global::Telegram.Bot.Args;
    using global::Telegram.Bot.Extensions.Polling;
    using global::Telegram.Bot.Types;
    using global::Telegram.Bot.Types.Enums;
    using global::Telegram.Bot.Types.ReplyMarkups;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Telegram Bot client.
    /// </summary>
    public class TelegramBotDevService : ITelegramBotService, IDisposable
    {
        private readonly ILogger logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IUserContextService userContextService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotDevService"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        /// <param name="config"><see cref="BotConfiguration"/>.</param>
        /// <param name="userContextService"><see cref="IUserContextService"/>.</param>
        /// <param name="scopeFactory"><see cref="IServiceScopeFactory"/>.</param>
        public TelegramBotDevService(
            ILogger<TelegramBotDevService> logger,
            BotConfiguration config,
            IUserContextService userContextService,
            IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.userContextService = userContextService;
            this.scopeFactory = scopeFactory;

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

            var me = this.Client.GetMeAsync().Result;
            this.logger.LogInformation($"Bot {me.Id}-{me.FirstName} is active.");

            this.Client.StartReceiving(new DefaultUpdateHandler(this.HandleUpdateAsync, this.HandleErrorAsync));
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

        private async Task HandleUpdateAsync(Update update, CancellationToken ct)
        {
            using var scope = this.scopeFactory.CreateScope();
            var context = (MarketplaceContext)scope.ServiceProvider.GetService(typeof(MarketplaceContext));

            var (userId, command, messageId) = UpdateHelpers.GetUserAndCommand(update);
            var user = await context.Users.FirstOrDefaultAsync(u => u.TelegramId == userId);
            var userContext = this.userContextService.GetUserContext(user, userId);

            PersonifiedUpdate personifiedUpdate = new PersonifiedUpdate()
            {
                Update = update,
                UserContext = userContext,
                Command = command,
                MessageId = messageId,
            };

            var updateHandler = (IBotUpdateService)scope.ServiceProvider.GetService(typeof(IBotUpdateService));
            await updateHandler.ProceedUpdate(personifiedUpdate);
        }

        private Task HandleErrorAsync(Exception ex, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}