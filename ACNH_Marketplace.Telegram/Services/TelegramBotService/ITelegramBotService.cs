// <copyright file="ITelegramBotService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services.BotService
{
    using System.Threading.Tasks;
    using global::Telegram.Bot;
    using global::Telegram.Bot.Types;
    using global::Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Telegram bot interface.
    /// </summary>
    public interface ITelegramBotService
    {
        /// <summary>
        /// Gets telegram client instance.
        /// </summary>
        TelegramBotClient Client { get; }

        /// <summary>
        /// Disgard telegram client webhook.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Edit message using Telegram API.
        /// If no messageId provided, execute <see cref="SendMessageAsync(int, string, int, IReplyMarkup)"/>.
        /// </summary>
        /// <param name="userId">Reciever user id.</param>
        /// <param name="messageId">Edit message id.</param>
        /// <param name="message">Mesasge text.</param>
        /// <param name="replyMarkup">Reply markup (custom keyboard).</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Message> EditMessageAsync(int userId, int? messageId, string message = null, InlineKeyboardMarkup replyMarkup = null);

        /// <summary>
        /// Send text message to Telegram API.
        /// </summary>
        /// <param name="userId">Reciever user id.</param>
        /// <param name="message">Mesasge text.</param>
        /// <param name="replyTo">Reply to message id.</param>
        /// <param name="replyMarkup">Reply markup (custom keyboard).</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Message> SendMessageAsync(int userId, string message, int replyTo = 0, IReplyMarkup replyMarkup = null);
    }
}