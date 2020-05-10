using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Enums;
using ACNH_Marketplace.Telegram.Services;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Telegram.Commands.CommandBase
{
    public abstract class BaseCommand : ICommand
    {
        protected readonly TelegramBotClient _client;
        protected readonly MarketplaceContext _context;
        protected readonly UserContext _userContext;
        protected readonly string _command;
        protected readonly UserStateEnum _userState;

        public BaseCommand(IBotService botService, MarketplaceContext context, UserContext userContext, string command)
        {
            _client = botService.Client;
            _context = context;
            _userContext = userContext;
            _command = command;
            _userState = _userContext.GetContext<UserStateEnum>(UserContextEnum.UserState);
        }

        public abstract Task Execute(Update update);
    }
}
