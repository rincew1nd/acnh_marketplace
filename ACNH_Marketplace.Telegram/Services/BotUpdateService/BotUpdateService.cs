using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Commands.CommandBase;
using ACNH_Marketplace.Telegram.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Telegram.Services
{
    public class BotUpdateService : IBotUpdateService
    {
        private readonly ICommandRouterService _commandRouter;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BotUpdateService> _logger;

        public BotUpdateService(ICommandRouterService crs, IServiceScopeFactory scopeFactory, ILogger<BotUpdateService> logger)
        {
            _commandRouter = crs;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task ProceedUpdate(PersonifiedUpdate update)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var type = _commandRouter.FindCommand(update);
                    var command = (ICommand)scope.ServiceProvider.GetService(type);
                    await command.Execute(update);
                }
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
