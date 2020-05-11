using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Commands.CommandBase;
using ACNH_Marketplace.Telegram.Enums;
using ACNH_Marketplace.Telegram.Services;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = ACNH_Marketplace.DataBase.Models.User;

namespace ACNH_Marketplace.Telegram.Commands
{
    public class WelcomeCommand : BaseCommand
    {
        private const string WelcomeMessage = @"Befor we start:
For registration you should provide your in game name (IGN) and island name.";

        private const string RegistrationConfirmation = @"Does it correct:
IGN - {0}
Island Name - {1}
Timezone(UTC) - {2}";
        private const string MainMenuMessage = @"Host or visit turnip exchange?";

        public WelcomeCommand(IBotService botService, MarketplaceContext context, UserContext userContext, string command) :
            base(botService, context, userContext, command)
        {
        }

        public override async Task Execute(Update update)
        {
            _update = update;

            switch (update.Type)
            {
                case UpdateType.Message:
                    switch (_userState)
                    {
                        case UserStateEnum.Default:
                            await SendMenu();
                            break;
                        case UserStateEnum.ConfirmRegistration:
                        case UserStateEnum.Welcome:
                            await SendWelcomeMessage(_userState);
                            break;
                        case UserStateEnum.EnteringIGName:
                            await SetIGN();
                            break;
                        case UserStateEnum.EnteringIslandName:
                            await SetIslandName();
                            break;
                        case UserStateEnum.EnteringUTC:
                            await SetTimeZone();
                            break;
                    }
                    break;
                case UpdateType.CallbackQuery:
                    await UpdateMenu();
                    break;
            }
        }

        private async Task SendMenu()
        {
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.Default);
            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: MainMenuMessage,
                replyMarkup: new InlineKeyboardMarkup(
                    new[] {
                        new InlineKeyboardButton() { Text = "Host turnip exchange", CallbackData = "/HostTurnip" },
                        new InlineKeyboardButton() { Text = "Visit turnip exchange", CallbackData = "/VisitTurnip" }
                    }
                )
            );
        }

        private async Task UpdateMenu()
        {
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.Default);
            await _client.EditMessageTextAsync(
                chatId: _userContext.UserId,
                messageId: _update.CallbackQuery.Message.MessageId,
                text: MainMenuMessage,
                replyMarkup: new InlineKeyboardMarkup(
                    new[] {
                        new InlineKeyboardButton() { Text = "Host turnip exchange", CallbackData = "/HostTurnip" },
                        new InlineKeyboardButton() { Text = "Visit turnip exchange", CallbackData = "/VisitTurnip" }
                    }
                )
            );
        }

        private async Task SendWelcomeMessage(UserStateEnum userState)
        {
            if (userState == UserStateEnum.Welcome)
            {
                await _client.SendTextMessageAsync(
                    chatId: _userContext.UserId,
                    text: string.Format(WelcomeMessage)
                );
            }

            if (_command == "Yes" && userState == UserStateEnum.ConfirmRegistration)
            {
                _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.Default);
                await CreateOrUpdateUser();
                await SendMenu();
            }
            else
            {
                _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringIGName);

                await _client.SendTextMessageAsync(
                    chatId: _userContext.UserId,
                    text: "Enter IGN:",
                    replyMarkup: new ForceReplyMarkup()
                );
            }
        }

        private async Task SetIGN()
        {
            _userContext.SetContext(UserContextEnum.InGameName, _command);
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringIslandName);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Enter island name:",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        private async Task SetIslandName(bool again = false)
        {
            if (!again)
            {
                _userContext.SetContext(UserContextEnum.IslandName, _command);
                _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringUTC);
            }

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Enter your timezone (-14 < UTC±0 < 14):",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        public async Task SetTimeZone()
        {
            var result = int.TryParse(_command, out var timezone);
            if (!result || timezone < -14 || timezone > 14)
            {
                await _client.SendTextMessageAsync(
                    chatId: _userContext.UserId,
                    text: $"'{_command}' is not a number. Only numbers from -14 to 14 acceptable."
                );
                await SetIslandName(true);
            }
            else
            {
                _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.ConfirmRegistration);
                _userContext.SetContext(UserContextEnum.UTC, timezone);

                var ign = _userContext.GetContext<string>(UserContextEnum.InGameName);
                var islandName = _userContext.GetContext<string>(UserContextEnum.IslandName);

                await _client.SendTextMessageAsync(
                    chatId: _userContext.UserId,
                    text: string.Format(RegistrationConfirmation, ign, islandName, _command),
                    replyMarkup: new ReplyKeyboardMarkup(
                        new KeyboardButton[][]
                        {
                            new KeyboardButton[] { "Yes", "No" }
                        },
                        true, true
                    )
                );
            }
        }

        private async Task CreateOrUpdateUser()
        {
            var ign = _userContext.GetContext<string>(UserContextEnum.InGameName);
            var islandName = _userContext.GetContext<string>(UserContextEnum.IslandName);
            var timezone = _userContext.GetContext<int>(UserContextEnum.UTC);

            var username = $"{_update.Message.From.FirstName} {_update.Message.From.LastName}";
            if (!string.IsNullOrWhiteSpace(_update.Message.From.Username))
            {
                username = $"{_update.Message.From.Username} ({username})";
            }

            var user = _context.Users.Find(_userContext.UserId);
            if (user == null)
            {
                _context.Add(new User()
                {
                    Id = _userContext.UserId,
                    InGameName = ign,
                    IslandName = islandName,
                    UserName = username,
                    Timezone = timezone,
                    LastActiveDate = DateTime.Now
                });
            }
            else
            {
                user.InGameName = ign;
                user.IslandName = islandName;
                user.UserName = username;
                user.Timezone = timezone;
                user.LastActiveDate = DateTime.Now;
                _context.Update(user);
            }
            await _context.SaveChangesAsync();
        }
    }
}
