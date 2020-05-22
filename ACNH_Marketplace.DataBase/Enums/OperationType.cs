// <copyright file="OperationType.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// Operation type.
    /// </summary>
    public enum OperationType : int
    {
        /// <summary>
        /// Default.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Turnip market hoster entry.
        /// </summary>
        TurnipMarket_Hoster,

        /// <summary>
        /// Turnip market visitor entry.
        /// </summary>
        TurnipMarket_Visitor,
    }
}
