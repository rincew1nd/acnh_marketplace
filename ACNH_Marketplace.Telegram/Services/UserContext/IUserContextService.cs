// <copyright file="IUserContextService.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Services
{
    using ACNH_Marketplace.DataBase.Models;

    /// <summary>
    /// User context provider interface.
    /// </summary>
    public interface IUserContextService
    {
        /// <summary>
        /// Get user context object.
        /// </summary>
        /// <param name="user"><see cref="User"/> object.</param>
        /// <param name="userId">Telegram user id.</param>
        /// <returns>Requests <see cref="UserContext">UserContext</see> object.</returns>
        UserContext GetUserContext(User user, int userId);
    }
}