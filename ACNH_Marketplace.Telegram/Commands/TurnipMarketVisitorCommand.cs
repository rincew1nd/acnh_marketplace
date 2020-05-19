// <copyright file="TurnipMarketVisitorCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.DataBase.Models;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Services.BotService;
    using global::Telegram.Bot.Types.ReplyMarkups;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Turnip market hoster operations.
    /// </summary>
    public class TurnipMarketVisitorCommand : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnipMarketVisitorCommand"/> class.
        /// </summary>
        /// <param name="botService"><see cref="ITelegramBotService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext"/>.</param>
        public TurnipMarketVisitorCommand(ITelegramBotService botService, MarketplaceContext context)
            : base(botService, context)
        {
        }

        private PersonifiedUpdate Update { get; set; }

        /// <inheritdoc/>
        public override async Task<OperationExecutionResult> Execute(PersonifiedUpdate update)
        {
            this.Update = update;

            update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.TurnipMarketVisitor);

            var isCreating = update.UserContext.GetContext<bool>("CreatingTMV");
            var isEditing = update.UserContext.GetContext<bool>("ChangingTMV");

            if (update.Command == "/CreateTMV" || isCreating)
            {
                this.Update.UserContext.SetContext("CreatingTMV", true);
                await this.CreateTMV();
                return OperationExecutionResult.Success;
            }

            if (update.Command.StartsWith("/Change") || isEditing)
            {
                this.Update.UserContext.SetContext("ChangingTMV", true);
                await this.EditTMV();
                return OperationExecutionResult.Success;
            }

            await this.ManageTMV();
            return OperationExecutionResult.Success;
        }

        #region Operations
        private async Task ManageTMV()
        {
            var tmv = await this.Context.TurnipMarketVisitors
                .FirstOrDefaultAsync(tmh => tmh.UserId == this.Update.UserContext.UserId);

            if (tmv == null)
            {
                await this.Client.EditMessageAsync(
                   this.Update.UserContext.TelegramId,
                   this.Update.MessageId,
                   "No registered visitor applications found.",
                   new InlineKeyboardMarkup(
                        new[]
                        {
                            new[] { new InlineKeyboardButton() { CallbackData = "/CreateTMV", Text = "Create visitor application" } },
                            new[] { new InlineKeyboardButton() { CallbackData = "/BackTMMM", Text = "<- Back" } },
                        }));
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Description - {tmv.Description}");
            sb.AppendLine($"Price lower bound - {tmv.PriceLowerBound}");
            sb.AppendLine($"\nWhat do you wish to change?");

            this.Update.UserContext.SetContext("TMVId", tmv.Id);

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                sb.ToString(),
                new InlineKeyboardMarkup(
                    new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton() { CallbackData = "/ChangeDescription", Text = "Change description" },
                            new InlineKeyboardButton() { CallbackData = "/ChangeEntryFee", Text = "Change entry fee" },
                        },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeLowerBound", Text = "Change price lower bound" } },
                        new[] { new InlineKeyboardButton() { CallbackData = $"/FindHoster {tmv.Id}", Text = "Look for awailable hosters" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/BackTMMM", Text = "<- Back" } },
                    }));
        }

        private async Task CreateTMV()
        {
            var stage = this.Update.UserContext.GetContext<int>("CreationStage");
            switch (stage)
            {
                case 0:
                    this.Update.UserContext.SetContext("CreationStage", 1);
                    await this.EnteringPriceLowerBound();
                    break;
                case 1:
                    if (await this.ValidatePrice())
                    {
                        this.Update.UserContext.SetContext("CreationStage", 2);
                        await this.EnteringDescription();
                    }

                    break;
                case 2:
                    this.Update.UserContext.SetContext<string>("VisitorDescription", this.Update.Command);
                    await this.UpdateVisitor();
                    this.Update.UserContext.RemoveContext("CreationStage");
                    this.Update.UserContext.RemoveContext("CreatingTMV");
                    await this.ManageTMV();
                    break;
            }
        }

        private async Task EditTMV()
        {
            if (this.Update.Command == "/ChangeLowerBound")
            {
                this.Update.UserContext.SetContext("ChangingTMVType", 1);
                await this.EnteringPriceLowerBound();
                return;
            }
            else if (this.Update.Command == "/ChangeDescription")
            {
                this.Update.UserContext.SetContext("ChangingTMVType", 2);
                await this.EnteringDescription();
                return;
            }
            else
            {
                var type = this.Update.UserContext.GetContext<int>("ChangingTMVType");

                bool result;
                switch (type)
                {
                    case 1:
                        result = await this.ValidatePrice();
                        break;
                    case 2:
                        result = true;
                        this.Update.UserContext.SetContext<string>("VisitorDescription", this.Update.Command);
                        break;
                    default:
                        await this.ManageTMV();
                        this.Update.UserContext.RemoveContext("ChangingTMVType");
                        this.Update.UserContext.RemoveContext("ChangingTMV");
                        return;
                }

                if (result)
                {
                    await this.UpdateVisitor();
                    this.Update.UserContext.RemoveContext("ChangingTMVType");
                    this.Update.UserContext.RemoveContext("ChangingTMV");
                    await this.ManageTMV();
                }
            }
        }
        #endregion

        #region Entering data messages
        private async Task EnteringPriceLowerBound()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Lower bound for turnip price to look for.\nEnter numer in range from 0 to 1000:");
        }

        private async Task EnteringDescription()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Enter visit application description:");
        }
        #endregion

        #region Validating data
        private async Task<bool> ValidatePrice()
        {
            if (!int.TryParse(this.Update.Command, out var price))
            {
                await this.Client.EditMessageAsync(
                    this.Update.UserContext.TelegramId,
                    this.Update.MessageId,
                    $"Invalid price format '{this.Update.Command}' != '\\d+'.");
                await this.EnteringPriceLowerBound();
                return false;
            }

            if (price < 0 || price > 1000)
            {
                await this.Client.EditMessageAsync(
                    this.Update.UserContext.TelegramId,
                    this.Update.MessageId,
                    $"Impossile turnip price.");
                await this.EnteringPriceLowerBound();
                return false;
            }

            this.Update.UserContext.SetContext("VisitorPriceLowerBound", price);
            return true;
        }
        #endregion

        #region Database operations
        private async Task UpdateVisitor()
        {
            var tmvId = this.Update.UserContext.GetContext<Guid>("TMVId");
            var price = this.Update.UserContext.GetContext<int>("VisitorPriceLowerBound");
            var description = this.Update.UserContext.GetContext<string>("VisitorDescription");

            var tmv = await this.Context.TurnipMarketVisitors
                .FirstOrDefaultAsync(tmh => tmh.Id == tmvId && tmh.UserId == this.Update.UserContext.UserId);

            if (tmv != null)
            {
                tmv.PriceLowerBound = price == default ? tmv.PriceLowerBound : price;
                tmv.Description = string.IsNullOrWhiteSpace(description) ? tmv.Description : description;
                this.Context.Update(tmv);
            }

            if (tmv == null)
            {
                tmv = new TurnipMarketVisitor()
                {
                    UserId = this.Update.UserContext.UserId,
                    PriceLowerBound = price,
                    Description = description,
                };
                await this.Context.AddAsync(tmv);
            }

            await this.Context.SaveChangesAsync();

            this.Update.UserContext.RemoveContext("TMVId");
            this.Update.UserContext.RemoveContext("VisitorPriceLowerBound");
            this.Update.UserContext.RemoveContext("VisitorDescription");
        }
        #endregion
    }
}
