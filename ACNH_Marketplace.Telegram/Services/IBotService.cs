using Telegram.Bot;

namespace ACNH_Marketplace.Telegram.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}