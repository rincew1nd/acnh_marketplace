using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Enums;
using ACNH_Marketplace.Telegram.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Telegram.Services
{
    public class BotUpdateService : IBotUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<BotUpdateService> _logger;
        private UserContextProvider _ucProvider;
        private MarketplaceContext _context;

        public BotUpdateService(
            IBotService botService, UserContextProvider ucProvider,
            MarketplaceContext context, ILogger<BotUpdateService> logger)
        {
            _botService = botService;
            _ucProvider = ucProvider;
            _context = context;
            _logger = logger;
        }

        public async Task ProceedUpdate(Update update)
        {
            try
            {
                var (userId, command) = UpdateHelpers.GetUserAndCommand(update);
                var user = await _context.Users.FindAsync(userId);
                var userContext = _ucProvider.GetUserContext(user, userId);
                var userState = userContext.GetContext<UserStateEnum>(UserContextEnum.UserState);

                var type = CommandHelpers.GetCommandType(userState, command)[0];
                var commandObj = CommandHelpers.CreateCommand(type, _botService, _context, userContext, command);

                await commandObj.Execute(update);
            }
            catch (Exception ex)
            {
                if (ex is CommandNotFoundException) { }
                else
                    _logger.LogError(ex, "Unhandled exception");
            }
        }
    }
}
