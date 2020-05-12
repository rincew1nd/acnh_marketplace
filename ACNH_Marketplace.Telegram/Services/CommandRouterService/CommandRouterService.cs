using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Commands;
using ACNH_Marketplace.Telegram.Commands.CommandBase;
using ACNH_Marketplace.Telegram.Enums;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ACNH_Marketplace.Telegram.Services
{
    public class CommandRouterService : ICommandRouterService
    {
        private readonly Dictionary<UserStateEnum, SortedList<string, string>> _routes;
        private readonly ILogger _logger;

        private readonly IBotService _botService;

        public CommandRouterService(IBotService botService, IConfiguration config, ILogger<CommandRouterService> logger)
        {
            _botService = botService;

            _logger = logger;

            _routes = new Dictionary<UserStateEnum, SortedList<string, string>>();
            config.GetSection("CommandRoutes").Bind(_routes);
        }

        public Type FindCommand(PersonifiedUpdate update)
        {
            var types = new List<System.Type>();

            var userState = update.Context.GetContext<UserStateEnum>(UserContextEnum.UserState);

            if (_routes.ContainsKey(userState))
            {
                foreach (var commandPattern in _routes[userState])
                {
                    if (Regex.IsMatch(update.Command, commandPattern.Key, RegexOptions.IgnoreCase))
                    {
                        types.Add(Type.GetType(commandPattern.Value));
                    }
                }
            }

            if (types.Count > 1)
                _logger.LogWarning($"Found more than one routes for command - {update.Command}");

            return !types.Any() ? typeof(WelcomeCommand) : types.First();
        }
    }
}
