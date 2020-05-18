// <copyright file="MainMenuCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.DataBase.Models;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Services;
    using ACNH_Marketplace.Telegram.Services.BotService;
    using global::Telegram.Bot.Types.ReplyMarkups;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Command for main page.
    /// </summary>
    public class MainMenuCommand : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuCommand"/> class.
        /// </summary>
        /// <param name="botService"><see cref="ITelegramBotService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext">Database context</see>.</param>
        public MainMenuCommand(ITelegramBotService botService, MarketplaceContext context)
            : base(botService, context)
        {
        }

        private PersonifiedUpdate Update { get; set; }

        /// <inheritdoc/>
        public override async Task<OperationExecutionResult> Execute(PersonifiedUpdate update)
        {
            this.Update = update;
            update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.MainPage);

            if (update.Command.Equals("/ProfileMain", StringComparison.OrdinalIgnoreCase))
            {
                update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.ProfileMain);
                return OperationExecutionResult.Reroute;
            }

            if (update.Command.Equals("/ImActive", StringComparison.OrdinalIgnoreCase))
            {
                await this.ExtendUserActivity();
                return OperationExecutionResult.Success;
            }

            await this.Client.EditMessageAsync(
                update.UserContext.TelegramId,
                update.MessageId,
                "What do you wish for right now?",
                new InlineKeyboardMarkup(new[]
                {
                    new[] { new InlineKeyboardButton() { CallbackData = "/TurnipMarket", Text = "Turnip market" } },
                    new[] { new InlineKeyboardButton() { CallbackData = "/ProfileMain", Text = "Edit profile" } },
                    new[] { new InlineKeyboardButton() { CallbackData = "/ImActive", Text = "I'm active right now!" } },
                }));
            return OperationExecutionResult.Success;
        }

        private async Task ExtendUserActivity()
        {
            var user = await this.Context.Users
                .FirstOrDefaultAsync(u => u.TelegramId == this.Update.UserContext.TelegramId);
            if (user != null)
            {
                user.LastActiveDate = DateTime.Now;
                this.Context.Update(user);
                await this.Context.SaveChangesAsync();

                await this.Client.SendMessageAsync(this.Update.UserContext.TelegramId, "Last active date updated!");
            }
        }
    }
}
