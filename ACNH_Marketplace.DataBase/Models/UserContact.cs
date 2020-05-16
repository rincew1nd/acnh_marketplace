// <copyright file="UserContact.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ACNH_Marketplace.DataBase.Enums;

    /// <summary>
    /// User contacts.
    /// </summary>
    public class UserContact
    {
        /// <summary>
        /// Gets or sets record primary key.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets user contact type.
        /// </summary>
        public UserContactType Type { get; set; }

        /// <summary>
        /// Gets or sets contact name.
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// <see cref="User"/> Gets or sets id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// <see cref="User"/> Gets or sets object.
        /// </summary>
        public User User { get; set; }
    }
}
