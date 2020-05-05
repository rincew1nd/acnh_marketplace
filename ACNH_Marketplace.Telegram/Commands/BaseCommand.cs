using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.DataBase.Models;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace ACNH_Marketplace.Telegram.Commands
{
    public abstract class BaseCommand
    {
        protected readonly TelegramBotClient _client;
        protected readonly MarketplaceContext _context;
        protected readonly User _user;

        public BaseCommand(TelegramBotClient client, MarketplaceContext context, User user)
        {
            _client = client;
            _context = context;
            _user = user;
        }

        public virtual async Task Execute(MessageEventArgs e, bool edited)
        {
            throw new NotImplementedException();
        }

        public virtual async Task Execute(ReceiveErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual async Task Execute(CallbackQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual async Task Execute(InlineQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual async Task Execute(ChosenInlineResultEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
