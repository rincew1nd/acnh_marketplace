using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.DataBase.Models;
using ACNH_Marketplace.Telegram.Commands.CommandBase;
using ACNH_Marketplace.Telegram.Enums;
using ACNH_Marketplace.Telegram.Helpers;
using ACNH_Marketplace.Telegram.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ACNH_Marketplace.Telegram.Commands
{
    public class HostTurnipExchangeCommand : BaseCommand
    {
        private static string HostDescription = "From: {0}\nTurnip Cost: {1}\nEnd date: {2}\nEntry fee:\n{3}\n\n{4}";
        private static string RegistrationHostDescription = "From: {0}\nTurnip Cost: {1}\nEnd date: {2}\n\n{3}";

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
                    else if (_command.StartsWith("/NewHTE"))
                        await NewHTEDate();
                    else if (_command == "/EditHTETurnipCost")
                        await EditHTETurnipCost();
                    else if (_command == "/EditHTEDate")
                        await EditHTEDate();
                    else if (_command == "/EditHTEDescription")
                        await EditHTEDescription();
                    //else if (_command == "/EditHTEFee")
                    //    await EditHTEFee();
                    else if (_command == "/EditHTEDelete")
                        await EditHTEDelete();
                    else if (_command.StartsWith("/EditHTE"))
                        await EditHTE();
                    break;
                case UpdateType.Message:
                    switch(_userState)
                    {
                        case UserStateEnum.EnteringHTEDate:
                            await NewHTEPrice();
                            break;
                        case UserStateEnum.EnteringHTEPrice:
                            await NewHTEDescription();
                            break;
                        case UserStateEnum.EnteringHTEDescription:
                            await NewHTEConfirm();
                            break;
                        case UserStateEnum.ConfirmHTERegistration:
                            await NewHTEDate();
                            break;
                        case UserStateEnum.EditHTEDate:
                        case UserStateEnum.EditHTEDescription:
                        case UserStateEnum.EditHTEPrice:
                            await SaveEditHTE();
                            await EditHTE();
                            break;
                    }
                    break;
            }
        }

        #region Init message
        private async Task InitHTE()
        {
            NullContext(true);
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
        #endregion

        #region HostTurnipExchange Edit
        private async Task EditHTE()
        {
            var guid = _userContext.GetContext<Guid?>(UserContextEnum.EditHTEId);
            if (guid.HasValue && guid != Guid.Empty)
            {
                await EditHTE(guid.Value);
                return;
            }
            else
            {
                if (_command.Split(' ').Length > 1 && Guid.TryParse(_command.Split(' ')[1], out var guidTemp))
                {
                    _userContext.SetContext(UserContextEnum.EditHTEId, (Guid?)guidTemp);
                    await EditHTE(guidTemp);
                    return;
                }
            }

            NullContext(true);

            var utc = _userContext.GetContext<int>(UserContextEnum.UTC);
            var hostsInfo = _context.TurnipMarketHosters
                .Where(tmh => tmh.UserId == _userContext.UserId && tmh.ExpirationDate > DateTime.Now)
                .OrderBy(tmh => tmh.ExpirationDate);

            var keyboardButtons = new List<List<InlineKeyboardButton>>();
            foreach (var hostInfo in hostsInfo)
            {
                keyboardButtons.Add(new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton()
                    {
                        Text = $"{hostInfo.ExpirationDate.ToUserDate(utc)}. Turnip cost {hostInfo.TurnipCost}",
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

        private async Task EditHTE(Guid hostId)
        {
            var hostInfo = _context.TurnipMarketHosters
                .Include(tmh => tmh.User)
                .Include(tmh => tmh.Fee)
                .FirstOrDefault(tmh => tmh.Id == hostId);

            FillContext(hostInfo);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text:
                    string.Format(HostDescription,
                        $"{hostInfo.User.InGameName} {hostInfo.User.IslandName}",
                        hostInfo.TurnipCost,
                        $"{hostInfo.ExpirationDate:dd.MM.yyyy}",
                        hostInfo.Message,
                        string.Join("\n", hostInfo.Fee.Select(fee => $"{fee.Count}x {fee.FeeType.GetDescription()} ({fee.Description})"))
                    ),
                replyMarkup: new InlineKeyboardMarkup(
                    new[] {
                        new[] { new InlineKeyboardButton() { Text = "Change turnip cost", CallbackData = "/EditHTETurnipCost" } },
                        new[] { new InlineKeyboardButton() { Text = "Change date", CallbackData = "/EditHTEDate" } },
                        new[] { new InlineKeyboardButton() { Text = "Change description", CallbackData = "/EditHTEDescription" } },
                        new[] { new InlineKeyboardButton() { Text = "Change entry fee", CallbackData = "/EditHTEFee" } },
                        new[] { new InlineKeyboardButton() { Text = "Delete exchange record", CallbackData = "/EditHTEFee" } },
                        new[] { new InlineKeyboardButton() { Text = "<= Back", CallbackData = "/HostTurnip" } }
                    })
            );
        }

        private async Task EditHTEDate()
        {
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EditHTEDate);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Enter date when exchange will end:\n(Format 'dd.MM.yyyy HH' example '14.02.2020 20)",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task EditHTETurnipCost()
        {
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EditHTEPrice);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Enter turnip cost:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task EditHTEDescription()
        {
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EditHTEDescription);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Enter description:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task SaveEditHTE()
        {
            switch (_userState)
            {
                case UserStateEnum.EditHTEDate:
                    if (!DateTime.TryParseExact(_command, "dd.MM.yyyy HH", null, DateTimeStyles.None, out var date))
                    {
                        await _client.SendTextMessageAsync(
                            chatId: _userContext.UserId,
                            text: $"Invalid date format {_command}."
                        );
                        await EditHTEDate();
                        return;
                    }
                    _userContext.SetContext(UserContextEnum.HTEDate, date);
                    await SaveOrUpdateHTE();
                    break;
                case UserStateEnum.EditHTEDescription:
                    _userContext.SetContext(UserContextEnum.HTEDescription, _command);
                    await SaveOrUpdateHTE();
                    break;
                case UserStateEnum.EditHTEPrice:
                    if (!int.TryParse(_command, out var price))
                    {
                        await _client.SendTextMessageAsync(
                            chatId: _userContext.UserId,
                            text: $"Invalid price format {_command}."
                        );
                        await EditHTEDate();
                        return;
                    }
                    _userContext.SetContext(UserContextEnum.HTEPrice, price);
                    await SaveOrUpdateHTE();
                    break;
            }
        }

        private async Task EditHTEDelete()
        {
            await DeleteHTE();
            _userContext.SetContext(UserContextEnum.EditHTEId, (Guid?)null);
            await EditHTE();
        }
        #endregion

        #region HostTurnipExchange Registration
        private async Task NewHTEDate()
        {
            if (_command == "Yes")
            {
                await SaveOrUpdateHTE();
                NullContext();
                await EditHTE();
            }
            else
            {
                _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringHTEDate);

                await _client.SendTextMessageAsync(
                    chatId: _userContext.UserId,
                    text: "Enter date when exchange will end:\n(Format 'dd.MM.yyyy HH' example '14.02.2020 20)",
                    replyMarkup: new ForceReplyMarkup()
                );
            }
        }

        private async Task NewHTEPrice(bool again = false)
        {
            if (!again)
            {
                if (!DateTime.TryParseExact(_command, "dd.MM.yyyy HH", null, DateTimeStyles.None, out var date))
                {
                    await _client.SendTextMessageAsync(
                        chatId: _userContext.UserId,
                        text: $"Invalid date format {_command}."
                    );
                    await NewHTEDate();
                    return;
                }

                _userContext.SetContext(UserContextEnum.HTEDate, date);
                _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringHTEPrice);
            }
            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Enter turnip cost:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task NewHTEDescription()
        {
            if (!int.TryParse(_command, out var price))
            {
                await _client.SendTextMessageAsync(
                    chatId: _userContext.UserId,
                    text: $"Invalid price format {_command}."
                );
                await NewHTEPrice(true);
                return;
            }

            _userContext.SetContext(UserContextEnum.HTEPrice, price);
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringHTEDescription);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Enter description:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task NewHTEConfirm()
        {
            _userContext.SetContext(UserContextEnum.HTEDescription, _command);
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.ConfirmHTERegistration);

            var username = _userContext.GetContext<string>(UserContextEnum.UserName);
            var date = _userContext.GetContext<DateTime>(UserContextEnum.HTEDate);
            var price = _userContext.GetContext<int>(UserContextEnum.HTEPrice);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: string.Format(RegistrationHostDescription, username, date, price, _command),
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                            new KeyboardButton[] { "Yes", "No" }
                    },
                    true, true
                )
            );
        }
        #endregion

        #region Database
        private async Task SaveOrUpdateHTE()
        {
            var id = _userContext.GetContext<Guid?>(UserContextEnum.EditHTEId);

            var utc = _userContext.GetContext<int>(UserContextEnum.UTC);
            var date = _userContext.GetContext<DateTime>(UserContextEnum.HTEDate);
            var price = _userContext.GetContext<int>(UserContextEnum.HTEPrice);
            var description = _userContext.GetContext<string>(UserContextEnum.HTEDescription);

            var hte = await _context.TurnipMarketHosters.FindAsync(id.Value);
            if (hte == null)
            {
                var tmh = new TurnipMarketHoster()
                {
                    RegistrationDate = DateTime.Now,
                    Message = description,
                    TurnipCost = price,
                    ExpirationDate = date.ToServerDate(utc),
                    UserId = _userContext.UserId
                };
                await _context.AddAsync(tmh);
                _userContext.SetContext(UserContextEnum.EditHTEId, (Guid?)tmh.Id);
            }
            else
            {
                hte = await _context.TurnipMarketHosters.FindAsync(id);
                hte.Message = description;
                hte.TurnipCost = price;
                hte.RegistrationDate = DateTime.Now;
                hte.ExpirationDate = date.ToServerDate(utc);
                _context.Update(hte);
            }
            await _context.SaveChangesAsync();
        }

        private async Task DeleteHTE()
        {
            var guid = _userContext.GetContext<Guid?>(UserContextEnum.EditHTEId);
            var hte = await _context.TurnipMarketHosters.FindAsync(guid.Value);
            _context.TurnipMarketHosters.Remove(hte);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Fill Context
        public void FillContext(TurnipMarketHoster tmh)
        {
            if (tmh != null)
            {
                _userContext.SetContext(UserContextEnum.HTEDate, tmh.ExpirationDate);
                _userContext.SetContext(UserContextEnum.HTEDescription, tmh.Message);
                _userContext.SetContext(UserContextEnum.HTEPrice, tmh.TurnipCost);
            }
        }

        public void NullContext(bool includingId = false)
        {
            _userContext.SetContext(UserContextEnum.HTEDate, default(DateTime));
            _userContext.SetContext(UserContextEnum.HTEDescription, "");
            _userContext.SetContext(UserContextEnum.HTEPrice, default(int));
            if (includingId)
                _userContext.SetContext(UserContextEnum.EditHTEId, default(Guid));
        }
        #endregion
    }
}
