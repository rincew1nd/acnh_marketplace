// <copyright file="TurnipMarketEntryFeeCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.DataBase.Enums;
    using ACNH_Marketplace.DataBase.Models;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Helpers;
    using ACNH_Marketplace.Telegram.Services.BotService;
    using global::Telegram.Bot.Types.ReplyMarkups;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Turnip market main menu.
    /// </summary>
    public class TurnipMarketEntryFeeCommand : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnipMarketEntryFeeCommand"/> class.
        /// </summary>
        /// <param name="botService"><see cref="ITelegramBotService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext"/>.</param>
        public TurnipMarketEntryFeeCommand(ITelegramBotService botService, MarketplaceContext context)
            : base(botService, context)
        {
        }

        private PersonifiedUpdate Update { get; set; }

        /// <inheritdoc/>
        public override async Task<OperationExecutionResult> Execute(PersonifiedUpdate update)
        {
            this.Update = update;

            update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.TurnipMarketEntryFee);

            var isCreating = update.UserContext.GetContext<bool>("CreatingTMEF");
            var isEditing = update.UserContext.GetContext<bool>("ChangingTMEF");

            try
            {
                if (update.Command == "/ChangeEntryFee" ||
                    update.Command.StartsWith("/ManageTMEF"))
                {
                    await this.ManageTMEF();
                    return OperationExecutionResult.Success;
                }

                if (update.Command == "/CreateTMEF" || isCreating)
                {
                    this.Update.UserContext.SetContext("CreatingTMEF", true);
                    await this.CreateTMEF();
                    return OperationExecutionResult.Success;
                }

                if (update.Command.StartsWith("/Change") || isEditing)
                {
                    this.Update.UserContext.SetContext("ChangingTMEF", true);
                    await this.EditTMEF();
                    return OperationExecutionResult.Success;
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    this.Update.Command = "/BackTMMM";
                    return OperationExecutionResult.Reroute;
                }

                throw ex;
            }

            return OperationExecutionResult.Success;
        }

        #region Operations
        private async Task ManageTMEF()
        {
            var commandParts = this.Update.Command.Split(' ');
            if (commandParts.Length > 1 && Guid.TryParse(commandParts[1], out var efId))
            {
                await this.ManageTMEF(efId);
                return;
            }

            var tmhId = this.Update.UserContext.GetContext<Guid>("TMHId");
            var tmvId = this.Update.UserContext.GetContext<Guid>("TMVId");

            var operationType = tmhId == default ? tmvId == default ?
                FeeOperationEnum.UnknownFee : FeeOperationEnum.VisitorEntryFee : FeeOperationEnum.HosterEntryFee;
            if (operationType == FeeOperationEnum.UnknownFee)
            {
                throw new ArgumentException();
            }

            this.Update.UserContext.SetContext("EntryFee_OperationType", operationType);

            IQueryable<EntryFee> efs = null;
            if (operationType == FeeOperationEnum.HosterEntryFee)
            {
                efs = this.Context.EntryFees.Where(ef => ef.TurnipMarketHosterId == tmhId);
            }
            else
            {
                efs = this.Context.EntryFees.Where(ef => ef.TurnipMarketVisitorId == tmvId);
            }

            var keyboard = new List<Tuple<string, string>[]>();
            foreach (var ef in efs)
            {
                keyboard.Add(new Tuple<string, string>[]
                {
                    new Tuple<string, string>(
                        $"/ManageTMEF {ef.Id}",
                        $"{ef.FeeType.GetDescription()} {ef.Count}"),
                });
            }

            keyboard.Add(new Tuple<string, string>[] { new Tuple<string, string>($"/CreateTMEF", "Create entry fee") });

            keyboard.Add(new Tuple<string, string>[]
            {
                new Tuple<string, string>(
                    (operationType == FeeOperationEnum.HosterEntryFee) ? $"/BackToHoster" : $"/BackToVisitor",
                    "<- Back"),
            });

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                $"Change entry fee for {operationType.GetDescription()}:",
                CommandHelper.BuildKeyboard(keyboard.ToArray()));
        }

        private async Task ManageTMEF(Guid efId)
        {
            var ef = await this.Context.EntryFees.FindAsync(efId);

            if (ef == null)
            {
                throw new ArgumentException();
            }

            this.Update.UserContext.SetContext("EntryFee_Id", efId);

            var keyboard = new Tuple<string, string>[][]
            {
                new Tuple<string, string>[]
                {
                    new Tuple<string, string>($"/ChangeType", "Change type"),
                    new Tuple<string, string>($"/ChangeCount", "Change count"),
                },
                new Tuple<string, string>[]
                {
                    new Tuple<string, string>($"/ChangeDescription", "Change description"),
                },
                new Tuple<string, string>[] { new Tuple<string, string>($"/ManageTMEF", "<- Back") },
            };

            var sb = new StringBuilder();
            sb.AppendLine($"Type - {ef.FeeType.GetDescription()}");
            sb.AppendLine($"Count - {ef.Count}");
            sb.AppendLine($"Description - {ef.Description}");
            sb.AppendLine($"\nWhat do you want to change:");

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                sb.ToString(),
                CommandHelper.BuildKeyboard(keyboard));
        }

        private async Task EditTMEF()
        {
            if (this.Update.Command == "/ChangeType")
            {
                this.Update.UserContext.SetContext("ChangingEFType", 1);
                await this.EnteringEFType();
                return;
            }
            else if (this.Update.Command == "/ChangeCount")
            {
                this.Update.UserContext.SetContext("ChangingEFType", 2);
                await this.EnteringCount();
                return;
            }
            else if (this.Update.Command == "/ChangeDescription")
            {
                this.Update.UserContext.SetContext("ChangingEFType", 3);
                await this.EnteringDescription();
                return;
            }
            else
            {
                var type = this.Update.UserContext.GetContext<int>("ChangingEFType");

                bool result;
                switch (type)
                {
                    case 1:
                        result = await this.ValidateEFType();
                        break;
                    case 2:
                        result = await this.ValidateCount();
                        break;
                    case 3:
                        result = true;
                        this.Update.UserContext.SetContext<string>("EntryFee_Description", this.Update.Command);
                        break;
                    default:
                        await this.ManageTMEF();
                        this.Update.UserContext.RemoveContext("ChangingEFType");
                        this.Update.UserContext.RemoveContext("ChangingEF");
                        return;
                }

                if (result)
                {
                    var guid = await this.UpdateEntryFee();
                    this.Update.UserContext.RemoveContext("ChangingEFType");
                    this.Update.UserContext.RemoveContext("ChangingTMEF");
                    await this.ManageTMEF(guid);
                }
            }
        }

        private async Task CreateTMEF()
        {
            var stage = this.Update.UserContext.GetContext<int>("CreationStage");
            switch (stage)
            {
                case 0:
                    this.Update.UserContext.SetContext("CreationStage", 1);
                    await this.EnteringEFType();
                    break;
                case 1:
                    if (await this.ValidateEFType())
                    {
                        this.Update.UserContext.SetContext("CreationStage", 2);
                        await this.EnteringCount();
                    }

                    break;
                case 2:
                    if (await this.ValidateCount())
                    {
                        this.Update.UserContext.SetContext("CreationStage", 3);
                        await this.EnteringDescription();
                    }

                    break;
                case 3:
                    this.Update.UserContext.SetContext<string>("EntryFee_Description", this.Update.Command);
                    var efId = await this.UpdateEntryFee();
                    this.Update.UserContext.RemoveContext("CreationStage");
                    this.Update.UserContext.RemoveContext("CreatingTMEF");
                    await this.ManageTMEF(efId);
                    break;
            }
        }
        #endregion

        #region Enter data messages
        private async Task EnteringEFType()
        {
            await this.Client.SendMessageAsync(
                this.Update.UserContext.TelegramId,
                "Choose entry fee type:",
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[]
                        {
                            new KeyboardButton() { Text = FeeType.NMT.GetDescription() },
                            new KeyboardButton() { Text = FeeType.StarFragment.GetDescription() },
                        },
                        new KeyboardButton[]
                        {
                            new KeyboardButton() { Text = FeeType.Recipe.GetDescription() },
                            new KeyboardButton() { Text = FeeType.Resource.GetDescription() },
                        },
                        new KeyboardButton[]
                        {
                            new KeyboardButton() { Text = FeeType.Money.GetDescription() },
                        },
                    },
                    true,
                    true));
        }

        private async Task EnteringCount()
        {
            await this.Client.SendMessageAsync(
                this.Update.UserContext.TelegramId,
                "Enter fee count (positive integer number):");
        }

        private async Task EnteringDescription()
        {
            await this.Client.SendMessageAsync(
                this.Update.UserContext.TelegramId,
                "Enter fee description:");
        }
        #endregion

        #region Validate data
        private async Task<bool> ValidateEFType()
        {
            try
            {
                var feeType = EnumString.ToDescription<FeeType>(this.Update.Command);
                this.Update.UserContext.SetContext("EntryFee_Type", feeType);
                return true;
            }
            catch (ArgumentException)
            {
                await this.Client.SendMessageAsync(
                    this.Update.UserContext.TelegramId,
                    "Please choose from keyboard items.");
                await this.EnteringEFType();
                return false;
            }
        }

        private async Task<bool> ValidateCount()
        {
            if (!int.TryParse(this.Update.Command, out var count))
            {
                await this.Client.SendMessageAsync(
                    this.Update.UserContext.TelegramId,
                    $"Error during parsing of number '{this.Update.Command}'");
                await this.EnteringCount();
                return false;
            }

            this.Update.UserContext.SetContext("EntryFee_Count", count);
            return true;
        }
        #endregion

        #region Update database
        private async Task<Guid> UpdateEntryFee()
        {
            var efId = this.Update.UserContext.GetContext<Guid>("EntryFee_Id");
            var type = this.Update.UserContext.GetContext<FeeType?>("EntryFee_Type");
            var count = this.Update.UserContext.GetContext<int>("EntryFee_Count");
            var description = this.Update.UserContext.GetContext<string>("EntryFee_Description");

            var ef = await this.Context.EntryFees
                .FirstOrDefaultAsync(ef => ef.Id == efId);

            if (ef != null)
            {
                ef.FeeType = type ?? ef.FeeType;
                ef.Count = count == default ? ef.Count : count;
                ef.Description = string.IsNullOrWhiteSpace(description) ? ef.Description : description;
                this.Context.Update(ef);
            }

            if (ef == null)
            {
                var tmhId = this.Update.UserContext.GetContext<Guid?>("TMHId");
                var tmvId = this.Update.UserContext.GetContext<Guid?>("TMVId");

                var operationType = tmhId == default ? tmvId == default ?
                    FeeOperationEnum.UnknownFee : FeeOperationEnum.VisitorEntryFee : FeeOperationEnum.HosterEntryFee;
                if (operationType == FeeOperationEnum.UnknownFee)
                {
                    throw new ArgumentException();
                }

                ef = new EntryFee()
                {
                    TurnipMarketHosterId = tmhId,
                    TurnipMarketVisitorId = tmvId,
                    FeeType = type.Value,
                    Count = count,
                    Description = description,
                };
                await this.Context.AddAsync(ef);
            }

            await this.Context.SaveChangesAsync();

            this.Update.UserContext.RemoveContext("EntryFee_Id");
            this.Update.UserContext.RemoveContext("EntryFee_Type");
            this.Update.UserContext.RemoveContext("EntryFee_Count");
            this.Update.UserContext.RemoveContext("EntryFee_Description");

            return ef.Id;
        }
        #endregion
    }
}
