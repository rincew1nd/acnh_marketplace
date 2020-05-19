// <copyright file="TurnipMarketFinderCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands
{
    using System;
    using System.Linq;
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

    /// <summary>
    /// Turnip market main menu.
    /// </summary>
    public class TurnipMarketFinderCommand : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnipMarketFinderCommand"/> class.
        /// </summary>
        /// <param name="botService"><see cref="ITelegramBotService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext"/>.</param>
        public TurnipMarketFinderCommand(ITelegramBotService botService, MarketplaceContext context)
            : base(botService, context)
        {
        }

        private PersonifiedUpdate Update { get; set; }

        /// <inheritdoc/>
        public override async Task<OperationExecutionResult> Execute(PersonifiedUpdate update)
        {
            this.Update = update;
            update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.TurnipMarketFinder);

            var backToTMMM = false;
            if (this.Update.Command.StartsWith("/FindHoster"))
            {
                backToTMMM = await this.FindHoster();
            }
            else if (this.Update.Command.StartsWith("/FindVisitor"))
            {
                backToTMMM = await this.FindVisitor();
            }
            else if (this.Update.Command.StartsWith("/HosterDescription"))
            {
                backToTMMM = await this.ViewHosterDescription();
            }
            else if (this.Update.Command.StartsWith("/VisitorDescription"))
            {
                backToTMMM = await this.ViewVisitorDescription();
            }

            if (!backToTMMM)
            {
                return OperationExecutionResult.Success;
            }

            this.Update.UserContext.RemoveContext("HosterId");
            this.Update.UserContext.RemoveContext("VisitorId");
            update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.TurnipMarket);
            this.Update.Command = "/BackTMMM";
            return OperationExecutionResult.Reroute;
        }

        private async Task<bool> FindHoster()
        {
            int skip = default;

            var commandParts = this.Update.Command.Split(' ');
            if (commandParts.Count() < 2 || !Guid.TryParse(commandParts[1], out var visitorId))
            {
                if (commandParts.Count() > 2)
                {
                    int.TryParse(commandParts[2], out skip);
                }

                return true;
            }

            this.Update.UserContext.SetContext("VisitorId", visitorId);

            var tmv = await this.Context.TurnipMarketVisitors.FindAsync(visitorId);

            var tmh = this.Context.TurnipMarketHosters
                .Include(tmh => tmh.User)
                .Include(tmh => tmh.Fee)
                .Where(tmh => tmh.User.LastActiveDate > DateTime.Now.AddHours(-2) &&
                              tmh.BeginingDate > DateTime.Now &&
                              tmh.ExpirationDate < DateTime.Now &&
                              tmh.Price > tmv.PriceLowerBound &&
                              tmh.UserId != this.Update.UserContext.UserId)
                .OrderByDescending(tmh => tmh.Price)
                .Skip(skip)
                .Take(10);

            var keyboard = tmh
                .Select(tmh => new[]
                {
                    new Tuple<string, string>(
                        $"/HosterDescription {tmh.Id}",
                        $"{DateTimeConverter.ToUserDate(tmh.ExpirationDate, this.Update.UserContext.Timezone):dd.MM.yyyy HH} - {tmh.Price}"),
                }).ToList();

            if (keyboard.Count >= 10)
            {
                keyboard.Append(new[] { new Tuple<string, string>($"/FindHoster {visitorId} {skip + 10}", "Next page >>") });
            }

            if (skip > 0)
            {
                keyboard.Append(new[] { new Tuple<string, string>($"/FindHoster {visitorId} {skip - 10}", "<< Previous page") });
            }

            keyboard.Append(new[] { new Tuple<string, string>("/BackTMMM", "<- Back") });

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Here is a list of currently available hosters for your visitor application:",
                CommandHelper.BuildKeyboard(keyboard.ToArray()));

            return false;
        }

        private async Task<bool> FindVisitor()
        {
            int skip = default;

            var commandParts = this.Update.Command.Split(' ');
            if (commandParts.Count() < 2 || !Guid.TryParse(commandParts[1], out var hosterId))
            {
                if (commandParts.Count() > 2)
                {
                    int.TryParse(commandParts[2], out skip);
                }

                return true;
            }

            this.Update.UserContext.SetContext("HosterId", hosterId);

            var tmh = await this.Context.TurnipMarketHosters.FindAsync(hosterId);

            var tmv = this.Context.TurnipMarketVisitors
                .Include(tmv => tmv.User)
                .Include(tmv => tmv.Fee)
                .Where(tmv => tmv.User.LastActiveDate > DateTime.Now.AddHours(-2) &&
                              tmv.PriceLowerBound < tmh.Price &&
                              tmv.UserId != this.Update.UserContext.UserId)
                .OrderBy(tmv => tmv.PriceLowerBound)
                .Skip(skip)
                .Take(10);

            var keyboard = tmv
                .Select(tmv => new[]
                {
                    new Tuple<string, string>(
                        $"/VisitorDescription {tmv.Id}",
                        $"Wanted price - {tmv.PriceLowerBound}"),
                }).ToList();

            if (keyboard.Count >= 10)
            {
                keyboard.Append(new[] { new Tuple<string, string>($"/FindVisitor {hosterId} {skip + 10}", "Next page >>") });
            }

            if (skip > 0)
            {
                keyboard.Append(new[] { new Tuple<string, string>($"/FindVisitor {hosterId} {skip - 10}", "<< Previous page") });
            }

            keyboard.Append(new[] { new Tuple<string, string>("/BackTMMM", "<- Back") });

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Here is a list of currently available visitor for your hosted market:",
                CommandHelper.BuildKeyboard(keyboard.ToArray()));

            return false;
        }

        private async Task<bool> ViewHosterDescription()
        {
            var commandParts = this.Update.Command.Split(' ');
            if (commandParts.Count() < 2 || !Guid.TryParse(commandParts[1], out var hosterId))
            {
                return true;
            }

            var id = this.Update.UserContext.GetContext<Guid>("HosterId");

            var tmh = await this.Context.TurnipMarketHosters
                .Include(tmh => tmh.User)
                .Include(tmh => tmh.Fee)
                .FirstOrDefaultAsync(tmh => tmh.Id == hosterId);

            var sb = new StringBuilder();
            sb.AppendLine(this.GetUserInfo(tmh.User));
            sb.AppendLine($"Available from - {DateTimeConverter.ToUserDate(tmh.BeginingDate, this.Update.UserContext.Timezone)}");
            sb.AppendLine($"Expires - {DateTimeConverter.ToUserDate(tmh.ExpirationDate, this.Update.UserContext.Timezone)}");
            sb.AppendLine($"Turnip price - {tmh.Price}");
            sb.AppendLine($"Description - {tmh.Description}");

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                sb.ToString(),
                CommandHelper.BuildKeyboard(
                    new[] { new[] { new Tuple<string, string>($"/FindHoster {id}", "<- Back") } }));

            return false;
        }

        private async Task<bool> ViewVisitorDescription()
        {
            var commandParts = this.Update.Command.Split(' ');
            if (commandParts.Count() < 2 || !Guid.TryParse(commandParts[1], out var visitorId))
            {
                return true;
            }

            var id = this.Update.UserContext.GetContext<Guid>("VisitorId");

            var tmv = await this.Context.TurnipMarketVisitors
                .Include(tmv => tmv.User)
                .Include(tmh => tmh.Fee)
                .FirstOrDefaultAsync(tmv => tmv.Id == visitorId);

            var sb = new StringBuilder();
            sb.AppendLine(this.GetUserInfo(tmv.User));
            sb.AppendLine($"Description - {tmv.Description}");
            sb.AppendLine($"Price lower bound - {tmv.PriceLowerBound}");

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                sb.ToString(),
                CommandHelper.BuildKeyboard(
                    new[] { new[] { new Tuple<string, string>($"/FindVisitor {id}", "<- Back") } }));

            return false;
        }

        private string GetUserInfo(User user)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{user.InGameName} from {user.IslandName}");
            sb.AppendLine($"Contacts:");
            sb.AppendLine($"\t\t{string.Join("\n\t\t", user.UserContacts.Select(uc => $"{uc.Type} - {uc.Contact}"))}");
            return sb.ToString();
        }
    }
}
