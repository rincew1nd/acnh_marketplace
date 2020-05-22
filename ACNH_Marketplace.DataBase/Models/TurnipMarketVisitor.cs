// <copyright file="TurnipMarketVisitor.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Turnip market visitor record.
    /// </summary>
    public class TurnipMarketVisitor
    {
        /// <summary>
        /// Gets or sets record primary key.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets hosters lower bounds for turnip price.
        /// </summary>
        public int PriceLowerBound { get; set; }

        /// <summary>
        /// Gets or sets turnip application description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets application's <see cref="User"/> id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets application's <see cref="User"/> object.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets collection of <see cref="EntryFees"/> to enter island.
        /// </summary>
        public IList<TurnipMarketEntryFee> EntryFees { get; set; }
    }
}
