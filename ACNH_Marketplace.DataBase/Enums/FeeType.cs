// <copyright file="FeeType.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// Island entry fee types.
    /// </summary>
    public enum FeeType : int
    {
        /// <summary>
        /// Default.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Bells.
        /// </summary>
        [Description("Bells")]
        Money,

        /// <summary>
        /// Nook miles ticket.
        /// </summary>
        [Description("Nook Miles Tickets")]
        NMT,

        /// <summary>
        /// Star fragments.
        /// </summary>
        [Description("Star Fragment")]
        StarFragment,

        /// <summary>
        /// Any kind of recipe (see fee description).
        /// </summary>
        [Description("Recipe")]
        Recipe,

        /// <summary>
        /// Any kind of resource (see fee description).
        /// </summary>
        [Description("Resource")]
        Resource,
    }
}
