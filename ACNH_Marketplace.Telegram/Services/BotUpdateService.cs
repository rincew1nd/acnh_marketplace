using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Telegram.Services
{
    public class BotUpdateService : IBotUpdateService
    {
        private readonly ILogger<BotUpdateService> _logger;

        private readonly UserContextProvider _ucProvider;
        private readonly MarketplaceContext _context;
        private readonly CommandRouterService _commandRouter;

        public BotUpdateService(UserContextProvider ucProvider, MarketplaceContext context,
            CommandRouterService crs, ILogger<BotUpdateService> logger)
        {
            _ucProvider = ucProvider;
            _context = context;
            _commandRouter = crs;
            _logger = logger;
        }

        public async Task ProceedUpdate(Update update)
        {
            try
            {
                var (userId, command) = UpdateHelpers.GetUserAndCommand(update);
                var user = await _context.Users.FindAsync(userId);
                var userContext = _ucProvider.GetUserContext(user, userId);
                await _commandRouter.FindCommand(_context, userContext, command).Execute(update);
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
