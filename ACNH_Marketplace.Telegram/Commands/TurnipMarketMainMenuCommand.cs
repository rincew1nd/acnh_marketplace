// <copyright file="TurnipMarketMainMenuCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands
{
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Services.BotService;
    using global::Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Turnip market main menu.
    /// </summary>
    public class TurnipMarketMainMenuCommand : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnipMarketMainMenuCommand"/> class.
        /// </summary>
        /// <param name="botService"><see cref="ITelegramBotService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext"/>.</param>
        public TurnipMarketMainMenuCommand(ITelegramBotService botService, MarketplaceContext context)
            : base(botService, context)
        {
        }

        /// <inheritdoc/>
        public override async Task<OperationExecutionResult> Execute(PersonifiedUpdate update)
        {
            update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.TurnipMarket);
            update.UserContext.RemoveContext("TMHId");
            update.UserContext.RemoveContext("TMVId");

            await this.Client.EditMessageAsync(
                update.UserContext.TelegramId,
                update.MessageId,
                "What do you wish for right now?",
                new InlineKeyboardMarkup(
                    new[]
                    {
                        new[] { new InlineKeyboardButton() { CallbackData = "/ManageTMH", Text = "Manage hosted markets" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/CreateTMH", Text = "Crate hosted market" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ManageTMV", Text = "Manage visit applications" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/BackMainMenu", Text = "<- Back" } },
                    }));

            return OperationExecutionResult.Success;
        }
    }
}
