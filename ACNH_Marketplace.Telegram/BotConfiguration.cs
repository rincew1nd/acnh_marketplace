// <copyright file="BotConfiguration.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram
{
    /// <summary>
    /// Object for storing bot configuration.
    /// </summary>
    public class BotConfiguration
    {
        /// <summary>
        /// Gets or sets telegram token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets telegram webhook uri.
        /// </summary>
        public string WebhookUri { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Proxy">proxy for telegram access</see>.
        /// </summary>
        public ProxyData Proxy { get; set; }

        /// <summary>
        /// Object for storing proxy configuration.
        /// </summary>
        public class ProxyData
        {
            /// <summary>
            /// Gets or sets proxy address.
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// Gets or sets proxy port.
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// Gets or sets proxy login.
            /// </summary>
            public string Login { get; set; }

            /// <summary>
            /// Gets or sets proxy password.
            /// </summary>
            public string Password { get; set; }
        }
    }
}
