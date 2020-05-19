// <copyright file="CommandHelper.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Helpers
{
    using System;
    using System.Collections.Generic;
    using global::Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Common operations for command in one place.
    /// </summary>
    public static class CommandHelper
    {
        /// <summary>
        /// Build <see cref="InlineKeyboardMarkup"/> two dimmension tuple of strings.
        /// </summary>
        /// <param name="keyboardButtons"><see cref="Tuple">two dimmension tuple of strings</see>.</param>
        /// <returns><see cref="InlineKeyboardMarkup"/>.</returns>
        public static InlineKeyboardMarkup BuildKeyboard(Tuple<string, string>[][] keyboardButtons)
        {
            var keyboard = new List<List<InlineKeyboardButton>>();
            foreach (var keyboardRow in keyboardButtons)
            {
                var keyboardRowObj = new List<InlineKeyboardButton>();
                foreach (var (callback, text) in keyboardRow)
                {
                    keyboardRowObj.Add(new InlineKeyboardButton()
                    {
                        CallbackData = callback,
                        Text = text,
                    });
                }

                keyboard.Add(keyboardRowObj);
            }

            return new InlineKeyboardMarkup(keyboard);
        }
    }
}
