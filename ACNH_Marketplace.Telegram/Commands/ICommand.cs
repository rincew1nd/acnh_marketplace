// <copyright file="ICommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands.CommandBase
{
    using System.Threading.Tasks;

    /// <summary>
    /// <see cref="ICommand"/> interface to process user update.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Process user update.
        /// </summary>
        /// <param name="update"><see cref="PersonifiedUpdate">Personified user update</see>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Execute(PersonifiedUpdate update);
    }
}