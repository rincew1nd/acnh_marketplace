// <copyright file="TurnipMarketHosterCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.DataBase.Models;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Helpers;
    using ACNH_Marketplace.Telegram.Services.BotService;
    using global::Telegram.Bot.Types.ReplyMarkups;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

    /// <summary>
    /// Turnip market hoster operations.
    /// </summary>
    public class TurnipMarketHosterCommand : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnipMarketHosterCommand"/> class.
        /// </summary>
        /// <param name="botService"><see cref="ITelegramBotService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext"/>.</param>
        public TurnipMarketHosterCommand(ITelegramBotService botService, MarketplaceContext context)
            : base(botService, context)
        {
        }

        private PersonifiedUpdate Update { get; set; }

        /// <inheritdoc/>
        public override async Task<OperationExecutionResult> Execute(PersonifiedUpdate update)
        {
            this.Update = update;

            update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.TurnipMarketHoster);

            var isCreating = update.UserContext.GetContext<bool>("CreatingTMH");
            var isEditing = update.UserContext.GetContext<bool>("ChangingTMH");

            if (update.Command == "/CreateTMH" || isCreating)
            {
                this.Update.UserContext.SetContext("CreatingTMH", true);
                await this.CreateTMH();
                return OperationExecutionResult.Success;
            }

            if (update.Command.StartsWith("/Change") || isEditing)
            {
                this.Update.UserContext.SetContext("ChangingTMH", true);
                await this.EditTMH();
                return OperationExecutionResult.Success;
            }

            if (update.Command == "/DeleteTMH")
            {
                await this.DeleteTMH();
            }

            if (update.Command == "/HosterCode")
            {
                await this.SendHosterCode();
                return OperationExecutionResult.Success;
            }

            await this.ManageTMH();
            return OperationExecutionResult.Success;
        }

        #region Operations
        private async Task ManageTMH()
        {
            if (this.Update.Command == "/BackManageTMH")
            {
                this.Update.UserContext.RemoveContext("TMHId");
            }

            var commandParts = this.Update.Command.Split(" ");
            if (commandParts.Length > 1)
            {
                if (Guid.TryParse(commandParts[1], out var tmhId_))
                {
                    await this.ManageTMH(tmhId_);
                    return;
                }
            }

            var tmhId = this.Update.UserContext.GetContext<Guid?>("TMHId");
            if (tmhId.HasValue)
            {
                await this.ManageTMH(tmhId.Value);
                return;
            }

            var tmh = this.Context.TurnipMarketHosters
                .Where(tmh => tmh.UserId == this.Update.UserContext.UserId &&
                              tmh.ExpirationDate > DateTime.Now)
                .Take(10)
                .OrderBy(tmh => tmh.ExpirationDate);

            var keyboardButtons = new List<InlineKeyboardButton[]>();
            foreach (var tmhInfo in tmh)
            {
                keyboardButtons.Add(new[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = $"{tmhInfo.ExpirationDate.ToUserDate(this.Update.UserContext.Timezone)}. Turnip price {tmhInfo.Price}",
                        CallbackData = $"/ManageTMH {tmhInfo.Id}",
                    },
                });
            }

            keyboardButtons.Add(new[]
            {
                new InlineKeyboardButton() { CallbackData = $"/BackTMMM", Text = $"<- Back", },
            });

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Choose hosted market to edit:",
                new InlineKeyboardMarkup(keyboardButtons));
        }

        private async Task ManageTMH(Guid id)
        {
            var tmh = await this.Context.TurnipMarketHosters
                .Include(tmh => tmh.EntryFees)
                .FirstOrDefaultAsync(tmh => tmh.Id == id);

            if (tmh == null)
            {
                await this.ManageTMH();
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Available from - {DateTimeConverter.ToUserDate(tmh.BeginingDate, this.Update.UserContext.Timezone)}");
            sb.AppendLine($"Expires - {DateTimeConverter.ToUserDate(tmh.ExpirationDate, this.Update.UserContext.Timezone)}");
            sb.AppendLine($"Turnip price - {tmh.Price}");
            sb.AppendLine($"Description - {tmh.Description}");
            sb.AppendLine($"Entry fee:");
            foreach (var fee in tmh.EntryFees)
            {
                sb.AppendLine($"\t\t{fee.FeeType.GetDescription()} {fee.Count} ({fee.Description})");
            }

            sb.AppendLine($"\nWhat do you wish to change?");

            this.Update.UserContext.SetContext("TMHId", id);

            var keyboard = new List<Tuple<string, string>[]>()
            {
                new[]
                {
                    new Tuple<string, string>("/ChangeDate", "Change market date"),
                    new Tuple<string, string>("/ChangePrice", "Change turnip price"),
                },
                new[]
                {
                    new Tuple<string, string>("/ChangeDescription", "Change description"),
                    new Tuple<string, string>("/ChangeEntryFee", "Change entry fee"),
                },
                new[]
                {
                    new Tuple<string, string>($"/DeleteTMH", "Delete market record"),
                    new Tuple<string, string>("/BackManageTMH", "<- Back"),
                },
            };

            if (tmh.BeginingDate <= DateTime.Now)
            {
                keyboard.Add(new[] { new Tuple<string, string>($"/FindVisitor {tmh.Id}", "Find visitors") });
            }

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                sb.ToString(),
                CommandHelper.BuildKeyboard(keyboard.ToArray()));
        }

        private async Task CreateTMH()
        {
            var stage = this.Update.UserContext.GetContext<int>("CreationStage");
            switch (stage)
            {
                case 0:
                    this.Update.UserContext.SetContext("CreationStage", 1);
                    await this.EnteringDate();
                    break;
                case 1:
                    if (await this.ValidateDate())
                    {
                        this.Update.UserContext.SetContext("CreationStage", 2);
                        await this.EnteringPrice();
                    }

                    break;
                case 2:
                    if (await this.ValidatePrice())
                    {
                        this.Update.UserContext.SetContext("CreationStage", 3);
                        await this.EnteringDescription();
                    }

                    break;
                case 3:
                    this.Update.UserContext.SetContext<string>("HosterDescription", this.Update.Command);
                    var tmhId = await this.UpdateHoster();
                    this.Update.UserContext.RemoveContext("CreationStage");
                    this.Update.UserContext.RemoveContext("CreatingTMH");
                    await this.ManageTMH(tmhId);
                    break;
            }
        }

        private async Task EditTMH()
        {
            if (this.Update.Command == "/ChangeDate")
            {
                this.Update.UserContext.SetContext("ChangingTMHType", 1);
                await this.EnteringDate();
                return;
            }
            else if (this.Update.Command == "/ChangePrice")
            {
                this.Update.UserContext.SetContext("ChangingTMHType", 2);
                await this.EnteringPrice();
                return;
            }
            else if (this.Update.Command == "/ChangeDescription")
            {
                this.Update.UserContext.SetContext("ChangingTMHType", 3);
                await this.EnteringDescription();
                return;
            }
            else
            {
                var type = this.Update.UserContext.GetContext<int>("ChangingTMHType");

                bool result;
                switch (type)
                {
                    case 1:
                        result = await this.ValidateDate();
                        break;
                    case 2:
                        result = await this.ValidatePrice();
                        break;
                    case 3:
                        result = true;
                        this.Update.UserContext.SetContext<string>("HosterDescription", this.Update.Command);
                        break;
                    default:
                        await this.ManageTMH();
                        this.Update.UserContext.RemoveContext("ChangingTMHType");
                        this.Update.UserContext.RemoveContext("ChangingTMH");
                        return;
                }

                if (result)
                {
                    var guid = await this.UpdateHoster();
                    this.Update.UserContext.RemoveContext("ChangingTMHType");
                    this.Update.UserContext.RemoveContext("ChangingTMH");
                    await this.ManageTMH(guid);
                }
            }
        }

        private async Task SendHosterCode()
        {
            var tmhId = this.Update.UserContext.GetContext<Guid>("TMHId");
            await this.Client.SendMessageAsync(
                this.Update.UserContext.TelegramId,
                $"Market code is - {tmhId}");
        }
        #endregion

        #region Entering data messages
        private async Task EnteringDate()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Enter the date until which the price will be available:\nFormat 'dd.MM.yyyy AM/PM'.\nFor examle '13.02.2020 PM' is between 13.02.2020 12:00 and 13.02.2020 22:00");
        }

        private async Task EnteringPrice()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Enter turnip price in range from 0 to 1000:");
        }

        private async Task EnteringDescription()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Enter market description:");
        }
        #endregion

        #region Validating data
        private async Task<bool> ValidateDate()
        {
            var dateStr = this.Update.Command.Replace("AM", "12").Replace("PM", "22");
            if (!DateTime.TryParseExact(dateStr, "dd.MM.yyyy HH", null, DateTimeStyles.None, out var date))
            {
                await this.Client.EditMessageAsync( 
                    this.Update.UserContext.TelegramId,
                    this.Update.MessageId,
                    $"Invalid date format '{dateStr}' != 'dd.MM.yyyy AM/PM'.");
                await this.EnteringDate();
                return false;
            }

            var serverDate = DateTimeConverter.ToServerDate(date, this.Update.UserContext.Timezone);
            if (serverDate < DateTime.Now || !serverDate.IsSameWeek())
            {
                await this.Client.EditMessageAsync(
                    this.Update.UserContext.TelegramId,
                    this.Update.MessageId,
                    $"Date ({date:dd.MM.yyyy HH} (UTC{this.Update.UserContext.Timezone})) can not be in the past and should be on the same week as current date.");
                await this.EnteringDate();
                return false;
            }

            this.Update.UserContext.SetContext("HosterDate", date);
            return true;
        }

        private async Task<bool> ValidatePrice()
        {
            if (!int.TryParse(this.Update.Command, out var price))
            {
                await this.Client.EditMessageAsync(
                    this.Update.UserContext.TelegramId,
                    this.Update.MessageId,
                    $"Invalid price format '{this.Update.Command}' != '\\d+'.");
                await this.EnteringPrice();
                return false;
            }

            if (price < 0 || price > 1000)
            {
                await this.Client.EditMessageAsync(
                    this.Update.UserContext.TelegramId,
                    this.Update.MessageId,
                    $"Impossile turnip price.");
                await this.EnteringPrice();
                return false;
            }

            this.Update.UserContext.SetContext("HosterPrice", price);
            return true;
        }
        #endregion

        #region Database operations
        private async Task<Guid> UpdateHoster()
        {
            DateTime beginningDate = default;

            var tmhId = this.Update.UserContext.GetContext<Guid>("TMHId");
            var expirationDate = this.Update.UserContext.GetContext<DateTime>("HosterDate");
            var price = this.Update.UserContext.GetContext<int>("HosterPrice");
            var description = this.Update.UserContext.GetContext<string>("HosterDescription");

            if (expirationDate != default)
            {
                beginningDate = expirationDate.Hour == 12 ? expirationDate.AddHours(-4) : expirationDate.AddHours(-10);
                beginningDate = DateTimeConverter.ToServerDate(beginningDate, this.Update.UserContext.Timezone);
                expirationDate = DateTimeConverter.ToServerDate(expirationDate, this.Update.UserContext.Timezone);
            }

            var tmh = await this.Context.TurnipMarketHosters
                .FirstOrDefaultAsync(tmh => tmh.Id == tmhId && tmh.UserId == this.Update.UserContext.UserId);

            if (tmh != null)
            {
                tmh.BeginingDate = beginningDate == default ? tmh.BeginingDate : beginningDate;
                tmh.ExpirationDate = expirationDate == default ? tmh.ExpirationDate : expirationDate;
                tmh.Price = price == default ? tmh.Price : price;
                tmh.Description = string.IsNullOrWhiteSpace(description) ? tmh.Description : description;
                this.Context.Update(tmh);
            }

            if (tmh == null)
            {
                tmh = new TurnipMarketHoster()
                {
                    UserId = this.Update.UserContext.UserId,
                    BeginingDate = beginningDate,
                    ExpirationDate = expirationDate,
                    Price = price,
                    Description = description,
                };
                await this.Context.AddAsync(tmh);
            }

            await this.Context.SaveChangesAsync();

            this.Update.UserContext.RemoveContext("TMHId");
            this.Update.UserContext.RemoveContext("HosterDate");
            this.Update.UserContext.RemoveContext("HosterPrice");
            this.Update.UserContext.RemoveContext("HosterDescription");

            return tmh.Id;
        }

        private async Task DeleteTMH()
        {
            var tmhId = this.Update.UserContext.GetContext<Guid?>("TMHId");

            if (tmhId.HasValue)
            {
                var tmh = await this.Context.TurnipMarketHosters
                    .Include(tmh => tmh.EntryFees)
                    .FirstOrDefaultAsync(tmh => tmh.Id == tmhId && tmh.UserId == this.Update.UserContext.UserId);
                if (tmh != null)
                {
                    this.Context.Remove(tmh);
                    await this.Context.SaveChangesAsync();

                    this.Update.UserContext.RemoveContext("TMHId");
                }
            }
        }
        #endregion
    }
}
