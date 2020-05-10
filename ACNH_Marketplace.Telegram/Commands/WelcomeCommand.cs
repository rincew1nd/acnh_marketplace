using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Commands.CommandBase;
using ACNH_Marketplace.Telegram.Enums;
using ACNH_Marketplace.Telegram.Services;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = ACNH_Marketplace.DataBase.Models.User;

namespace ACNH_Marketplace.Telegram._commands.Registration
{
    [Command("Default=^[^/].*;Welcome=.*;EnteringIGName=.*;EnteringIslandName=.*;ConfirmRegistration=.*")]
    public class WelcomeCommand : BaseCommand
    {
        private const string WelcomeMessage = @"Befor we start:
For registration you should provide your in game name (IGN) and island name.";

        private const string RegistrationConfirmation = @"Does it correct:
IGN - {0}
Island Name - {1}";

        private Update _update;

        public WelcomeCommand(IBotService botService, MarketplaceContext context, UserContext userContext, string command) :
            base(botService, context, userContext, command)
        {
        }

        public override async Task Execute(Update update)
        {
            _update = update;

            if (update.Message != null)
            {
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
                }
            }
        }

        private async Task SendMenu()
        {
            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Host or visit turnip exchange?",
                replyMarkup: new InlineKeyboardMarkup(
                    new[] {
                        new InlineKeyboardButton() { Text = "Host turnip exchange", CallbackData = "/hostturnip" },
                        new InlineKeyboardButton() { Text = "Visit turnip exchange", CallbackData = "/visitturnip" }
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
                await CreateUser();
                await SendMenu();
            }
            else
            {
                _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringIGName);

                await _client.SendTextMessageAsync(
                    chatId: _userContext.UserId,
                    text: "Enter IGN:"
                );
            }
        }

        private async Task SetIGN()
        {
            _userContext.SetContext(UserContextEnum.InGameName, _command);
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.EnteringIslandName);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Enter island name:"
            );
        }

        private async Task SetIslandName()
        {
            _userContext.SetContext(UserContextEnum.IslandName, _command);
            var ign = _userContext.GetContext<string>(UserContextEnum.InGameName);
            _userContext.SetContext(UserContextEnum.UserState, UserStateEnum.ConfirmRegistration);

            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: string.Format(RegistrationConfirmation, ign, _command),
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Yes", "No" }
                    }
                )
            );
        }

        private async Task CreateUser()
        {
            var ign = _userContext.GetContext<string>(UserContextEnum.InGameName);
            var islandName = _userContext.GetContext<string>(UserContextEnum.IslandName);

            _context.Add(new User()
            {
                Id = _userContext.UserId,
                InGameName = ign,
                IslandName = islandName,
                LastActiveDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }
    }
}
