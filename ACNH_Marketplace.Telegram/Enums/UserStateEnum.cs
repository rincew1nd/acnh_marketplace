// <copyright file="UserStateEnum.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Enums
{
    /// <summary>
    /// Possible user states.
    /// </summary>
    public enum UserStateEnum : int
    {
        /// <summary>
        /// Unknown user state.
        /// </summary>
        Unknown,

        /// <summary>
        /// User is not registered.
        /// </summary>
        NotRegistered,

        /// <summary>
        /// User on main page.
        /// </summary>
        MainPage,

        /// <summary>
        /// Main profile menu.
        /// </summary>
        ProfileMain,

        /// <summary>
        /// Edit profile menu.
        /// </summary>
        ProfileEdit,

        /// <summary>
        /// Turnip market main menu.
        /// </summary>
        TurnipMarket,

        /// <summary>
        /// Turnip market Hoster menu.
        /// </summary>
        TurnipMarketHoster,

        /// <summary>
        /// Turnip market Visitor menu.
        /// </summary>
        TurnipMarketVisitor,

        /// <summary>
        /// Find turnip markets or visitors.
        /// </summary>
        TurnipMarketFinder,

        /// <summary>
        /// Entry fee manager for entities.
        /// </summary>
        TurnipMarketEntryFee,
    }
}
