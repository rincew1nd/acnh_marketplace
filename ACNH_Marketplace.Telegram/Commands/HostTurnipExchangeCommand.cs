using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Commands.CommandBase;
using ACNH_Marketplace.Telegram.Enums;
using ACNH_Marketplace.Telegram.Services;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Telegram.Commands.Registration
{
    [Command("Default=^/hostturnip")]
    public class HostTurnipExchangeCommand : BaseCommand
    {
        public HostTurnipExchangeCommand(IBotService botService, MarketplaceContext context, UserContext userContext, string command) :
            base(botService, context, userContext, command)
        {
        }

        public override async Task Execute(Update update)
        {
            await _client.SendTextMessageAsync(
                chatId: _userContext.UserId,
                text: "Host turnip exchange"
            );
        }
    }
}
