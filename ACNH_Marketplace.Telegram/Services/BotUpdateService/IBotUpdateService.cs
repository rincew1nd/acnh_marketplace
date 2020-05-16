// <copyright file="IBotUpdateService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Bot service interface.
    /// </summary>
    public interface IBotUpdateService
    {
        /// <summary>
        /// Proceed update received by bot.
        /// </summary>
        /// <param name="update"><see cref="PersonifiedUpdate"/>.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task ProceedUpdate(PersonifiedUpdate update);
    }
}