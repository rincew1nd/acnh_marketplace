// <copyright file="EntryFeeOperationEnum.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    /// For which type of entity (hoster|visitor) entry fee edited.
    /// </summary>
    public enum FeeOperationEnum
    {
        /// <summary>
        /// Default enum value.
        /// </summary>
        UnknownFee,

        /// <summary>
        /// Entry fee for hoster entity
        /// </summary>
        [Description("hosted market")]
        HosterEntryFee,

        /// <summary>
        /// Entry fee for visitor entity
        /// </summary>
        [Description("visitor application")]
        VisitorEntryFee,
    }
}
