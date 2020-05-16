// <copyright file="EntryFee.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using ACNH_Marketplace.DataBase.Enums;

    /// <summary>
    /// Island entering fee record.
    /// </summary>
    public class EntryFee
    {
        /// <summary>
        /// Gets or sets record primary key.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets identifier of hoster.
        /// </summary>
        [ForeignKey("TurnipMarketHoster")]
        public Guid? TurnipMarketHosterId { get; set; }

        /// <summary>
        /// Gets or sets identifier of visitor.
        /// </summary>
        [ForeignKey("TurnipMarketVisitor")]
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
