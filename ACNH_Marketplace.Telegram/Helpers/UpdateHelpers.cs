using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ACNH_Marketplace.Telegram.Helpers
{
    public static class UpdateHelpers
    {
        public static (int userId, string command) GetUserAndCommand(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                case UpdateType.EditedMessage:
                    return (update.Message.From.Id, update.Message.Text);
                case UpdateType.InlineQuery:
                    return (update.InlineQuery.From.Id, update.InlineQuery.Query);
                case UpdateType.ChosenInlineResult:
                    return (update.ChosenInlineResult.From.Id, update.ChosenInlineResult.Query);
                case UpdateType.CallbackQuery:
                    return (update.CallbackQuery.From.Id, update.CallbackQuery.Data);
            }
            return (0, null);
        }
    }
}
