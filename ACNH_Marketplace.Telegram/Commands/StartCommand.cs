using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace ACNH_Marketplace.Telegram.Commands
{
    [Command("/start .+")]
    public class StartCommand : BaseCommand
    {
        public StartCommand(TelegramBotClient client, MarketplaceContext context, User user) : base(client, context, user)
        {
        }

        public override async Task Execute(MessageEventArgs e, bool edited)
        {
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat,
                text: "You said:\n" + e.Message.Text
            );
        }
    }
}
