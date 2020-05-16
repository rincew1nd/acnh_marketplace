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
        public static (int userId, string command, int? messageId) GetUserAndCommand(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                case UpdateType.EditedMessage:
                    return (update.Message.From.Id, update.Message.Text, null);
                case UpdateType.InlineQuery:
                    return (update.InlineQuery.From.Id, update.InlineQuery.Query, int.Parse(update.InlineQuery.Id));
                case UpdateType.ChosenInlineResult:
                    return (update.ChosenInlineResult.From.Id, update.ChosenInlineResult.Query, int.Parse(update.ChosenInlineResult.InlineMessageId));
                case UpdateType.CallbackQuery:
                    return (update.CallbackQuery.From.Id, update.CallbackQuery.Data, int.Parse(update.CallbackQuery.Id));
            }

            return (0, null, null);
        }
    }
}
