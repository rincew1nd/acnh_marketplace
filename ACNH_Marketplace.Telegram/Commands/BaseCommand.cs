// <copyright file="BaseCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands.CommandBase
{
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Services.BotService;

    /// <summary>
    /// Base <see cref="ICommand"/> implementation with <see cref="ITelegramBotService"/> and <see cref="MarketplaceContext"/>.
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="botService"><see cref="ITelegramBotService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext">Database context</see>.</param>
        public BaseCommand(ITelegramBotService botService, MarketplaceContext context)
        {
            this.Client = botService;
            this.Context = context;
        }

        /// <summary>
        /// Gets a bot service to use it to interact with user.
        /// </summary>
        protected ITelegramBotService Client { get; }

        /// <summary>
        /// Gets a database cotext.
        /// </summary>
        protected MarketplaceContext Context { get; }

        /// <inheritdoc/>
        public abstract Task<OperationExecutionResult> Execute(PersonifiedUpdate update);
    }
}
