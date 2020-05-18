// <copyright file="ICommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands.CommandBase
{
    using System.Threading.Tasks;
    using ACNH_Marketplace.Telegram.Enums;

    /// <summary>
    /// <see cref="ICommand"/> interface to process user update.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Process user update.
        /// </summary>
        /// <param name="update"><see cref="PersonifiedUpdate">Personified user update</see>.</param>
        /// <returns>A <see cref="Task"/> with operation execution result - <see cref="OperationExecutionResult"/>.</returns>
        Task<OperationExecutionResult> Execute(PersonifiedUpdate update);
    }
}