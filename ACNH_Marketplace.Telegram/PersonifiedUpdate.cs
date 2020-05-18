// <copyright file="PersonifiedUpdate.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram
{
    using ACNH_Marketplace.Telegram.Services;
    using global::Telegram.Bot.Types;

    /// <summary>
    /// Personified user update.
    /// Contains user context and current processing command.
    /// </summary>
    public class PersonifiedUpdate
    {
        /// <summary>
        /// Gets or sets <see cref="Services.UserContext">user context</see>.
        /// </summary>
        public UserContext UserContext { get; set; }

        /// <summary>
        /// Gets or sets current processing user command.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Update">bot update</see>.
        /// </summary>
        public Update Update { get; set; }

        /// <summary>
        /// Gets or sets message id.
        /// </summary>
        public int? MessageId { get; set; }
    }
}
