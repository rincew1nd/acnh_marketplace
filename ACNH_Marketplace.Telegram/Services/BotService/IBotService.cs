// <copyright file="IBotService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using global::Telegram.Bot;

    /// <summary>
    /// Interface for chat bot service.
    /// </summary>
    public interface IBotService
    {
        /// <summary>
        /// Gets telegram client.
        /// </summary>
        TelegramBotClient Client { get; }
    }
}