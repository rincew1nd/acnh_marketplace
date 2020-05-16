// <copyright file="ICommandRouterService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using System;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;

    /// <summary>
    /// Interface for command router based on <see cref="UserStateEnum"/> and command text.
    /// </summary>
    public interface ICommandRouterService
    {
        /// <summary>
        /// Find a <see cref="ICommand"/> based on <see cref="UserStateEnum"/> and command text.
        /// </summary>
        /// <param name="update"><see cref="PersonifiedUpdate">Personified update</see>.</param>
        /// <returns><see cref="ICommand"/> command.</returns>
        Type FindCommand(PersonifiedUpdate update);
    }
}