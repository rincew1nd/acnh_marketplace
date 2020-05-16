// <copyright file="CommandNotFoundException.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Helpers
{
    using System;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;

    /// <summary>
    /// <see cref="ICommand">Command</see> not found.
    /// </summary>
    public class CommandNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
        /// </summary>
        /// <param name="command">Command text.</param>
        public CommandNotFoundException(string command)
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets or sets command text.
        /// </summary>
        public string Command { get; set; }
    }
}
