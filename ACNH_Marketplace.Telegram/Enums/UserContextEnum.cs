// <copyright file="UserContextEnum.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Enums
{
    /// <summary>
    /// Available user context arguments.
    /// </summary>
    public enum UserContextEnum : int
    {
        /// <summary>
        /// Current user state.
        /// </summary>
        UserState,

        /// <summary>
        /// Users IGN.
        /// </summary>
        InGameName,

        /// <summary>
        /// Users island name.
        /// </summary>
        IslandName,

        /// <summary>
        /// Users timezone.
        /// </summary>
        Timezone,
    }
}
