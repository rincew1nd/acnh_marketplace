// <copyright file="ReviewType.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Enums
{
    /// <summary>
    /// User review type.
    /// </summary>
    public enum ReviewType : int
    {
        /// <summary>
        /// Default.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Turnip market review for hoster.
        /// </summary>
        TurnipHost = 1,

        /// <summary>
        /// Turnip market review for visitor.
        /// </summary>
        TurnipVisitor = 2,

        /// <summary>
        /// Default exchange review for both sides.
        /// </summary>
        Exchange = 3,
    }
}
