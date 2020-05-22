// <copyright file="UserReport.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using ACNH_Marketplace.DataBase.Enums;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    /// <summary>
    /// Reports against user.
    /// </summary>
    public class UserReport
    {
        /// <summary>
        /// Gets or sets record id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets reported user id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets reported user object.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets operation type.
        /// </summary>
        public OperationType OperationType { get; set; }

        /// <summary>
        /// Gets or sets operation id.
        /// </summary>
        public Guid? OperationId { get; set; }
    }
}
