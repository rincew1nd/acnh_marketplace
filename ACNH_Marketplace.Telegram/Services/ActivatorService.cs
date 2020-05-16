// <copyright file="ActivatorService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Hack for activating Telegram Webhook.
    /// </summary>
    public class ActivatorService : IHostedService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatorService"/> class.
        /// </summary>
        /// <param name="service">Telegram Bot Client.</param>
        #pragma warning disable IDE0060 // Remove unused parameter
        public ActivatorService(IBotService service)
        #pragma warning restore IDE0060 // Remove unused parameter
        {
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
