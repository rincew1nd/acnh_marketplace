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
                    return (update.InlineQuery.From.Id, update.InlineQuery.Query, null);
                case UpdateType.ChosenInlineResult:
                    return (update.ChosenInlineResult.From.Id, update.ChosenInlineResult.Query, null);
                case UpdateType.CallbackQuery:
                    return (update.CallbackQuery.From.Id, update.CallbackQuery.Data, update.CallbackQuery.Message.MessageId);
            }

            return (0, null, null);
        }

        /// <summary>
        /// Convert string to int without exceptions.
        /// </summary>
        /// <param name="value">Number in string.</param>
        /// <returns>Number or null.</returns>
        public static int? TryParseInt(string value)
        {
            if (int.TryParse(value, out var number))
            {
                return number;
            }

            return null;
        }
    }
}
