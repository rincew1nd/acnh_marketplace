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
        /// Gets or sets turnip market description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets visitors <see cref="User"/> id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets visitors <see cref="User"/> object.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets collection of <see cref="Fee"/> to enter island.
        /// </summary>
        public IList<EntryFee> Fee { get; set; }
    }
}
