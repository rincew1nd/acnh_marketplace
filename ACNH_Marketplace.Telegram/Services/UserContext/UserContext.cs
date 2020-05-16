// <copyright file="UserContext.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using System.Collections.Generic;
    using ACNH_Marketplace.DataBase.Models;
    using ACNH_Marketplace.Telegram.Enums;

    /// <summary>
    /// User context object.
    /// </summary>
    public class UserContext
    {
        // TODO: Add input dictionary.

        /// <summary>
        /// Dictionary with user attributes keyed by string.
        /// </summary>
        private readonly Dictionary<string, object> userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserContext"/> class.
        /// </summary>
        /// <param name="user"><see cref="User">User database record</see>.</param>
        /// <param name="userId">Telegram user id.</param>
        public UserContext(User user, int userId)
        {
            this.UserId = userId;
            if (user != null)
            {
                this.userContext = new Dictionary<string, object>()
                {
                    { UserContextEnum.UserState.ToString(), UserStateEnum.MainPage },
                    { UserContextEnum.InGameName.ToString(), user.InGameName },
                    { UserContextEnum.IslandName.ToString(), user.IslandName },
                    { UserContextEnum.Timezone.ToString(), user.Timezone },
                };
            }
            else
            {
                this.userContext = new Dictionary<string, object>()
                {
                    { UserContextEnum.UserState.ToString(), UserStateEnum.NotRegistered },
                };
            }
        }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Get user context by <see cref="UserContextEnum">attribute</see>.
        /// </summary>
        /// <typeparam name="T">Type of stored value.</typeparam>
        /// <param name="ucEnum"><see cref="UserContextEnum">Attribute</see>.</param>
        /// <returns>Attribute value.</returns>
        public T GetContext<T>(UserContextEnum ucEnum)
        {
            return this.GetContext<T>(ucEnum.ToString());
        }

        /// <summary>
        /// Set user context for <see cref="UserContextEnum">attribute</see>.
        /// </summary>
        /// <typeparam name="T">Type of stored value.</typeparam>
        /// <param name="ucEnum"><see cref="UserContextEnum">Attribute</see>.</param>
        /// <param name="obj">Attribute value.</param>
        public void SetContext<T>(UserContextEnum ucEnum, T obj)
        {
            this.SetContext<T>(ucEnum.ToString(), obj);
        }

        /// <summary>
        /// Get user context by <see cref="UserContextEnum">attribute</see>.
        /// </summary>
        /// <typeparam name="T">Type of stored value.</typeparam>
        /// <param name="name">Attribute name.</param>
        /// <returns>Attribute value.</returns>
        public T GetContext<T>(string name)
        {
            if (this.userContext.ContainsKey(name))
            {
                return (T)this.userContext[name];
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Set user context for <see cref="UserContextEnum">attribute</see>.
        /// </summary>
        /// <typeparam name="T">Type of stored value.</typeparam>
        /// <param name="name">Attribute name.</param>
        /// <param name="obj">Attribute value.</param>
        public void SetContext<T>(string name, T obj)
        {
            if (this.userContext.ContainsKey(name))
            {
                this.userContext[name] = obj;
            }
            else
            {
                this.userContext.Add(name, obj);
            }
        }

        /// <summary>
        /// Remove attribute from user context.
        /// </summary>
        /// <param name="ucEnum"><see cref="UserContextEnum">Attribute</see>.</param>
        public void RemoveContext(UserContextEnum ucEnum)
        {
            this.RemoveContext(ucEnum.ToString());
        }

        /// <summary>
        /// Remove attribute from user context.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        public void RemoveContext(string name)
        {
            if (this.userContext.ContainsKey(name))
            {
                this.userContext.Remove(name);
            }
        }
    }
}
