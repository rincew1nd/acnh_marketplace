// <copyright file="UpdateHelpers.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Helpers
{
    using global::Telegram.Bot.Types;
    using global::Telegram.Bot.Types.Enums;

    /// <summary>
    /// Telegram <see cref="Update"/> helpers.
    /// </summary>
    public static class UpdateHelpers
    {
        /// <summary>
        /// Get userId and command from <see cref="Update"/> object.
        /// </summary>
        /// <param name="update">Telegram <see cref="Update"/> object.</param>
        /// <returns>UserId and Command tuple.</returns>
        public static (int userId, string command) GetUserAndCommand(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                case UpdateType.EditedMessage:
                    return (update.Message.From.Id, update.Message.Text);
                case UpdateType.InlineQuery:
                    return (update.InlineQuery.From.Id, update.InlineQuery.Query);
                case UpdateType.ChosenInlineResult:
                    return (update.ChosenInlineResult.From.Id, update.ChosenInlineResult.Query);
                case UpdateType.CallbackQuery:
                    return (update.CallbackQuery.From.Id, update.CallbackQuery.Data);
            }

            return (0, null);
        }
    }
}
