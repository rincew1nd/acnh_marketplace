// <copyright file="BotUpdateService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using System;
    using System.Threading.Tasks;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Helpers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc/>
    public class BotUpdateService : IBotUpdateService
    {
        private readonly ICommandRouterService commandRouter;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<BotUpdateService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotUpdateService"/> class.
        /// </summary>
        /// <param name="crs"><see cref="ICommandRouterService"/>.</param>
        /// <param name="scopeFactory"><see cref="IServiceScopeFactory"/> for resolving commands.</param>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        public BotUpdateService(ICommandRouterService crs, IServiceScopeFactory scopeFactory, ILogger<BotUpdateService> logger)
        {
            this.commandRouter = crs;
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task ProceedUpdate(PersonifiedUpdate update)
        {
            try
            {
                var result = OperationExecutionResult.Reroute;
                do
                {
                    using var scope = this.scopeFactory.CreateScope();
                    var type = this.commandRouter.FindCommand(update);
                    var command = (ICommand)scope.ServiceProvider.GetService(type);
                    result = await command.Execute(update);
                }
                while (result == OperationExecutionResult.Reroute);
            }
            catch (Exception ex)
            {
                if (!(ex is CommandNotFoundException))
                {
                    this.logger.LogError(ex, "Unhandled exception");
                }
            }
        }
    }
}
