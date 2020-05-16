// <copyright file="UserReview.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ACNH_Marketplace.DataBase.Enums;

    /// <summary>
    /// Users reviews deals with other users.
    /// </summary>
    public class UserReview
    {
        /// <summary>
        /// Gets or sets record primary key.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets review type.
        /// </summary>
        public ReviewType Type { get; set; }

        /// <summary>
        /// Gets or sets review description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets deal satisfaction rating.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets reviewer <see cref="User"/> id.
        /// </summary>
        public Guid ReviewerId { get; set; }

        /// <summary>
        /// Gets or sets reviewer <see cref="User"/> object.
        /// </summary>
        public User Reviewer { get; set; }

        /// <summary>
        /// Gets or sets reviewed <see cref="User"/> id.
        /// </summary>
        public Guid ReviewedId { get; set; }

        /// <summary>
        /// Gets or sets reviewed <see cref="User"/> object.
        /// </summary>
        public User Reviewed { get; set; }
    }
}
