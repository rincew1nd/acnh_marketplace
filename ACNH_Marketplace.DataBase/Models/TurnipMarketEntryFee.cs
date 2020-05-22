// <copyright file="TurnipMarketEntryFee.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ACNH_Marketplace.DataBase.Enums;

    /// <summary>
    /// Island entering fee record.
    /// </summary>
    public class TurnipMarketEntryFee
    {
        /// <summary>
        /// Gets or sets record primary key.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets turnip market hoster record id.
        /// </summary>
        public Guid? TurnipMarketHosterId { get; set; }

        /// <summary>
        /// Gets or sets turnip market visitor record id.
        /// </summary>
        public Guid? TurnipMarketVisitorId { get; set; }

        /// <summary>
        /// Gets or sets fee type.
        /// </summary>
        public FeeType FeeType { get; set; }

        /// <summary>
        /// Gets or sets fee description if needed (for resources or recipes).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets bells price or item count.
        /// </summary>
        public int Count { get; set; }
    }
}
