// <copyright file="CommandRouterService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ACNH_Marketplace.Telegram.Enums;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc/>
    public class CommandRouterService : ICommandRouterService
    {
        private readonly Dictionary<UserStateEnum, SortedList<string, string>> routes;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRouterService"/> class.
        /// </summary>
        /// <param name="config"><see cref="IConfiguration"/>.</param>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        public CommandRouterService(IConfiguration config, ILogger<CommandRouterService> logger)
        {
            this.logger = logger;

            this.routes = new Dictionary<UserStateEnum, SortedList<string, string>>();
            config.GetSection("CommandRoutes").Bind(this.routes);
        }

        /// <inheritdoc/>
        public Type FindCommand(PersonifiedUpdate update)
        {
            var types = new List<System.Type>();

            var userState = update.Context.GetContext<UserStateEnum>(UserContextEnum.UserState);

            if (this.routes.ContainsKey(userState))
            {
                foreach (var commandPattern in this.routes[userState])
                {
                    if (Regex.IsMatch(update.Command, commandPattern.Key, RegexOptions.IgnoreCase))
                    {
                        types.Add(Type.GetType(commandPattern.Value));
                    }
                }
            }

            if (types.Count > 1)
            {
                this.logger.LogWarning($"Found more than one routes for command - {update.Command}");
            }

            return /*!types.Any() ? typeof(WelcomeCommand) :*/ types.First();
        }
    }
}
