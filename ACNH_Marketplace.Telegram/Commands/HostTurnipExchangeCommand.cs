﻿using ACNH_Marketplace.DataBase;
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
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ACNH_Marketplace.Telegram.Commands
{
    public class HostTurnipExchangeCommand : BaseCommand
    {
        private static string HostDescription = "From: {0}\nTurnip Cost: {1}\nEnd date: {2}\nEntry fee:\n{3}\n\n{4}";
        private static string RegistrationHostDescription = "From: {0}\nTurnip Cost: {1}\nEnd date: {2}\n\n{3}";

        private PersonifiedUpdate _update;
        private UserStateEnum _userState;


        public HostTurnipExchangeCommand(IBotService botService, MarketplaceContext context) : base(botService, context)
        {
        }

        public override async Task Execute(PersonifiedUpdate update)
        {
            _update = update;
            _userState = update.Context.GetContext<UserStateEnum>(UserContextEnum.UserState);

            switch (update.Update.Type)
            {
                case UpdateType.CallbackQuery:
                    if (update.Command.StartsWith("/HostTurnip"))
                        await InitHTE();
                    else if (update.Command.StartsWith("/NewHTE"))
                        await NewHTEDate();
                    else if (update.Command == "/EditHTETurnipCost")
                        await EditHTETurnipCost();
                    else if (update.Command == "/EditHTEDate")
                        await EditHTEDate();
                    else if (update.Command == "/EditHTEDescription")
                        await EditHTEDescription();
                    //else if (update.Command == "/EditHTEFee")
                    //    await EditHTEFee();
                    else if (update.Command == "/EditHTEDelete")
                        await EditHTEDelete();
                    else if (update.Command.StartsWith("/EditHTE"))
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
            _update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.HostTurnipExchange);
            await _client.EditMessageTextAsync(
                chatId: _update.Context.UserId,
                messageId: _update.Update.CallbackQuery.Message.MessageId,
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
            var guid = _update.Context.GetContext<Guid?>(UserContextEnum.EditHTEId);
            if (guid.HasValue && guid != Guid.Empty)
            {
                await EditHTE(guid.Value);
                return;
            }
            else
            {
                if (_update.Command.Split(' ').Length > 1 && Guid.TryParse(_update.Command.Split(' ')[1], out var guidTemp))
                {
                    _update.Context.SetContext(UserContextEnum.EditHTEId, (Guid?)guidTemp);
                    await EditHTE(guidTemp);
                    return;
                }
            }

            NullContext(true);

            var utc = _update.Context.GetContext<int>(UserContextEnum.UTC);
            var hostsInfo = _context.TurnipMarketHosters
                .Where(tmh => tmh.UserId == _update.Context.UserId && tmh.ExpirationDate > DateTime.Now)
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
                chatId: _update.Context.UserId,
                messageId: _update.Update.CallbackQuery.Message.MessageId,
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
                chatId: _update.Context.UserId,
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
                        new[] { new InlineKeyboardButton() { Text = "Delete exchange record", CallbackData = "/EditHTEDelete" } },
                        new[] { new InlineKeyboardButton() { Text = "<= Back", CallbackData = "/HostTurnip" } }
                    })
            );
        }

        private async Task EditHTEDate()
        {
            _update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.EditHTEDate);

            await _client.SendTextMessageAsync(
                chatId: _update.Context.UserId,
                text: "Enter date when exchange will end:\n(Format 'dd.MM.yyyy HH' example '14.02.2020 20)",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task EditHTETurnipCost()
        {
            _update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.EditHTEPrice);

            await _client.SendTextMessageAsync(
                chatId: _update.Context.UserId,
                text: "Enter turnip cost:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task EditHTEDescription()
        {
            _update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.EditHTEDescription);

            await _client.SendTextMessageAsync(
                chatId: _update.Context.UserId,
                text: "Enter description:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task SaveEditHTE()
        {
            switch (_userState)
            {
                case UserStateEnum.EditHTEDate:
                    if (!DateTime.TryParseExact(_update.Command, "dd.MM.yyyy HH", null, DateTimeStyles.None, out var date))
                    {
                        await _client.SendTextMessageAsync(
                            chatId: _update.Context.UserId,
                            text: $"Invalid date format {_update.Command}."
                        );
                        await EditHTEDate();
                        return;
                    }
                    _update.Context.SetContext(UserContextEnum.HTEDate, date);
                    await SaveOrUpdateHTE();
                    break;
                case UserStateEnum.EditHTEDescription:
                    _update.Context.SetContext(UserContextEnum.HTEDescription, _update.Command);
                    await SaveOrUpdateHTE();
                    break;
                case UserStateEnum.EditHTEPrice:
                    if (!int.TryParse(_update.Command, out var price))
                    {
                        await _client.SendTextMessageAsync(
                            chatId: _update.Context.UserId,
                            text: $"Invalid price format {_update.Command}."
                        );
                        await EditHTEDate();
                        return;
                    }
                    _update.Context.SetContext(UserContextEnum.HTEPrice, price);
                    await SaveOrUpdateHTE();
                    break;
            }
        }

        private async Task EditHTEDelete()
        {
            await DeleteHTE();
            _update.Context.SetContext(UserContextEnum.EditHTEId, (Guid?)null);
            await EditHTE();
        }
        #endregion

        #region HostTurnipExchange Registration
        private async Task NewHTEDate()
        {
            if (_update.Command == "Yes")
            {
                await SaveOrUpdateHTE();
                NullContext();
                await EditHTE();
            }
            else
            {
                _update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringHTEDate);

                await _client.SendTextMessageAsync(
                    chatId: _update.Context.UserId,
                    text: "Enter date when exchange will end:\n(Format 'dd.MM.yyyy HH' example '14.02.2020 20)",
                    replyMarkup: new ForceReplyMarkup()
                );
            }
        }

        private async Task NewHTEPrice(bool again = false)
        {
            if (!again)
            {
                if (!DateTime.TryParseExact(_update.Command, "dd.MM.yyyy HH", null, DateTimeStyles.None, out var date))
                {
                    await _client.SendTextMessageAsync(
                        chatId: _update.Context.UserId,
                        text: $"Invalid date format {_update.Command}."
                    );
                    await NewHTEDate();
                    return;
                }

                _update.Context.SetContext(UserContextEnum.HTEDate, date);
                _update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringHTEPrice);
            }
            await _client.SendTextMessageAsync(
                chatId: _update.Context.UserId,
                text: "Enter turnip cost:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task NewHTEDescription()
        {
            if (!int.TryParse(_update.Command, out var price))
            {
                await _client.SendTextMessageAsync(
                    chatId: _update.Context.UserId,
                    text: $"Invalid price format {_update.Command}."
                );
                await NewHTEPrice(true);
                return;
            }

            _update.Context.SetContext(UserContextEnum.HTEPrice, price);
            _update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringHTEDescription);

            await _client.SendTextMessageAsync(
                chatId: _update.Context.UserId,
                text: "Enter description:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task NewHTEConfirm()
        {
            _update.Context.SetContext(UserContextEnum.HTEDescription, _update.Command);
            _update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.ConfirmHTERegistration);

            var username = _update.Context.GetContext<string>(UserContextEnum.UserName);
            var date = _update.Context.GetContext<DateTime>(UserContextEnum.HTEDate);
            var price = _update.Context.GetContext<int>(UserContextEnum.HTEPrice);

            await _client.SendTextMessageAsync(
                chatId: _update.Context.UserId,
                text: string.Format(RegistrationHostDescription, username, date, price, _update.Command),
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
            var id = _update.Context.GetContext<Guid?>(UserContextEnum.EditHTEId);

            var utc = _update.Context.GetContext<int>(UserContextEnum.UTC);
            var date = _update.Context.GetContext<DateTime>(UserContextEnum.HTEDate);
            var price = _update.Context.GetContext<int>(UserContextEnum.HTEPrice);
            var description = _update.Context.GetContext<string>(UserContextEnum.HTEDescription);

            var hte = await _context.TurnipMarketHosters.FindAsync(id.Value);
            if (hte == null)
            {
                var tmh = new TurnipMarketHoster()
                {
                    RegistrationDate = DateTime.Now,
                    Message = description,
                    TurnipCost = price,
                    ExpirationDate = date.ToServerDate(utc),
                    UserId = _update.Context.UserId
                };
                await _context.AddAsync(tmh);
                _update.Context.SetContext(UserContextEnum.EditHTEId, (Guid?)tmh.Id);
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
            var guid = _update.Context.GetContext<Guid?>(UserContextEnum.EditHTEId);
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
                _update.Context.SetContext(UserContextEnum.HTEDate, tmh.ExpirationDate);
                _update.Context.SetContext(UserContextEnum.HTEDescription, tmh.Message);
                _update.Context.SetContext(UserContextEnum.HTEPrice, tmh.TurnipCost);
            }
        }

        public void NullContext(bool includingId = false)
        {
            _update.Context.SetContext(UserContextEnum.HTEDate, default(DateTime));
            _update.Context.SetContext(UserContextEnum.HTEDescription, "");
            _update.Context.SetContext(UserContextEnum.HTEPrice, default(int));
            if (includingId)
                _update.Context.SetContext(UserContextEnum.EditHTEId, default(Guid));
        }
        #endregion
    }
}
