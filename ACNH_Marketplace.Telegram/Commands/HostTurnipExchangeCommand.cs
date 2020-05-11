using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Commands.CommandBase;
using ACNH_Marketplace.Telegram.Enums;
using ACNH_Marketplace.Telegram.Helpers;
using ACNH_Marketplace.Telegram.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ACNH_Marketplace.Telegram.Commands
{
    public class HostTurnipExchangeCommand : BaseCommand
    {
        private static string HostDescription = "From: {0}\nTurnip Cost: {1}\nEnd date: {2}\n\n{3}\nEntry fee:\n{4}";

        public HostTurnipExchangeCommand(IBotService botService, MarketplaceContext context, UserContext userContext, string command) :
            base(botService, context, userContext, command)
        {
        }

        public override async Task Execute(Update update)
        {
            _update = update;
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    if (_command.StartsWith("/HostTurnip"))
                        await InitHTE();
                    else if (_command == "/EditHTE")
                        await EditHTE();
                    else if (_command.StartsWith("/EditHTE"))
                        await EditHTE(_command);
                    else if (_command.StartsWith("/NewHTE"))
                        await NewHTE();
                    break;
                case UpdateType.Message:
                    await _client.SendTextMessageAsync(
                        chatId: _userContext.UserId,
                        text: "Host turnip exchange"
                    );
                    break;
            }
        }

        private async Task InitHTE()
        {
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.HostTurnipExchange);
            await _client.EditMessageTextAsync(
                chatId: _userContext.UserId,
                messageId: _update.CallbackQuery.Message.MessageId,
                text: "Host turnip exchange menu:",
                replyMarkup: new InlineKeyboardMarkup(
                    new[] {
                        new[] { new InlineKeyboardButton() { Text = "Change previously hosted turnip exchanges", CallbackData = "/EditHTE" } },
                        new[] { new InlineKeyboardButton() { Text = "Host new turnip exchange", CallbackData = "/NewHTE" } },
                        new[] { new InlineKeyboardButton() { Text = "<= Back to main menu", CallbackData = "/BTWelcome" } }
                    })
            );
        }

        private async Task EditHTE()
        {
            var hostsInfo = _context.TurnipMarketHosters
                .Where(tmh => tmh.UserId == _userContext.UserId && tmh.ExpirationDate > DateTime.Now);

            var keyboardButtons = new List<List<InlineKeyboardButton>>();
            foreach (var hostInfo in hostsInfo)
            {
                keyboardButtons.Add(new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton()
                    {
                        Text = $"{hostInfo.ExpirationDate.Date}. Turnip cost {hostInfo.TurnipCost}",
                        CallbackData = $"/EditHTE {hostInfo.Id}"
                    }
                });
            }
            if (!hostsInfo.Any())
            {
                keyboardButtons.Add(new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton() { Text = $"No active exchanges found", CallbackData = "/HostTurnip" }
                });
            }
            keyboardButtons.Add(new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "<= Back", CallbackData = "/HostTurnip" }
            });

            await _client.EditMessageReplyMarkupAsync(
                chatId: _userContext.UserId,
                messageId: _update.CallbackQuery.Message.MessageId,
                replyMarkup: new InlineKeyboardMarkup(keyboardButtons)
            );
        }

        private async Task EditHTE(string command)
        {
            var hostId = Guid.Parse(command.Split(' ')[1]);
            var hostInfo = _context.TurnipMarketHosters
                .Include(tmh => tmh.User)
                .Include(tmh => tmh.Fee)
                .FirstOrDefault(tmh => tmh.Id == hostId);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text:
                    string.Format(HostDescription,
                        $"{hostInfo.User.InGameName} {hostInfo.User.IslandName}",
                        $"{hostInfo.ExpirationDate:dd.MM.yyyy}",
                        hostInfo.TurnipCost,
                        hostInfo.Message,
                        string.Join("\n", hostInfo.Fee.Select(fee => $"{fee.Count}x {fee.FeeType.GetDescription()} ({fee.Description})"))
                    ),
                replyMarkup: new InlineKeyboardMarkup(
                    new[] {
                        new[] { new InlineKeyboardButton() { Text = "Change turnip cost", CallbackData = "/EditHTETurnipCost" } },
                        new[] { new InlineKeyboardButton() { Text = "Change date", CallbackData = "/EditHTEDate" } },
                        new[] { new InlineKeyboardButton() { Text = "Change description", CallbackData = "/EditHTEDescription" } },
                        new[] { new InlineKeyboardButton() { Text = "Change entry fee", CallbackData = "/EditHTEFee" } },
                        new[] { new InlineKeyboardButton() { Text = "<= Back", CallbackData = "/HostTurnip" } }
                    })
            );
        }

        private Task NewHTE()
        {
            throw new NotImplementedException();
        }
    }
}
