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

        public BaseCommand(IBotService botService, MarketplaceContext context)
        {
            _client = botService.Client;
            _context = context;
        }

        public abstract Task Execute(PersonifiedUpdate update);
    }
}
