// <copyright file="UserContextService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using System.Collections.Generic;
    using ACNH_Marketplace.DataBase.Models;

    /// <inheritdoc/>
    public class UserContextService : IUserContextService
    {
        private readonly Dictionary<int, UserContext> userContexts;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserContextService"/> class.
        /// </summary>
        public UserContextService()
        {
            this.userContexts = new Dictionary<int, UserContext>();
        }

        /// <inheritdoc/>
        public UserContext GetUserContext(User user, int userId)
        {
            if (this.userContexts.ContainsKey(userId))
            {
                return this.userContexts[userId];
            }

            this.userContexts.Add(userId, new UserContext(user, userId));
            return this.userContexts[userId];
        }
    }
}
