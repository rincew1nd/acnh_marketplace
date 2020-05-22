// <copyright file="User.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// User record.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets users primary key.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets users telegram id.
        /// </summary>
        public int TelegramId { get; set; }

        /// <summary>
        /// Gets or sets users in game name (IGN).
        /// </summary>
        public string InGameName { get; set; }

        /// <summary>
        /// Gets or sets users island name.
        /// </summary>
        public string IslandName { get; set; }

        /// <summary>
        /// Gets or sets users UTC timezone.
        /// </summary>
        public int Timezone { get; set; }

        /// <summary>
        /// Gets or sets users last active time.
        /// </summary>
        public DateTime LastActiveDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users is banned or not.
        /// </summary>
        public bool IsBanned { get; set; }

        /// <summary>
        /// Gets or sets users reviews.
        /// </summary>
        public IList<UserReview> UserReviews { get; set; }

        /// <summary>
        /// Gets or sets collection of <see cref="TurnipMarketHoster">hosted turnip markets</see> from user.
        /// </summary>
        public IList<TurnipMarketHoster> Hosts { get; set; }

        /// <summary>
        /// Gets or sets <see cref="TurnipMarketVisitor">turnip market visit application</see> from user.
        /// </summary>
        public TurnipMarketVisitor Visit { get; set; }

        /// <summary>
        /// Gets or sets user contacts.
        /// </summary>
        public IList<UserContact> UserContacts { get; set; }

        /// <summary>
        /// Gets or sets user reports.
        /// </summary>
        public IList<UserReport> UserReports { get; set; }
    }
}
