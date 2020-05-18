// <copyright file="TurnipMarketHoster.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Turnip market hoster record.
    /// </summary>
    public class TurnipMarketHoster
    {
        /// <summary>
        /// Gets or sets record primary key.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets date when market will be active.
        /// </summary>
        public DateTime BeginingDate { get; set; }

        /// <summary>
        /// Gets or sets date when market will expire.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets turnip market description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets turnips market price.
        /// </summary>
        public int TurnipPrice { get; set; }

        /// <summary>
        /// Gets or sets hosters <see cref="User"/> id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets hosters <see cref="User"/> object.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets collection of <see cref="Fee"/> to enter island.
        /// </summary>
        public IList<EntryFee> Fee { get; set; }
    }
}
