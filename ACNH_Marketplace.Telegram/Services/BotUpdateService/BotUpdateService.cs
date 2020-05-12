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
        private readonly CommandRouterService _commandRouter;

        public BotUpdateService(CommandRouterService crs, ILogger<BotUpdateService> logger)
        {
            _commandRouter = crs;
            _logger = logger;
        }

        public async Task ProceedUpdate(PersonifiedUpdate update)
        {
            try
            {
                var command = _commandRouter.FindCommand(update);
                await command.Execute(update);
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
