// <copyright file="DateTimeConverter.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Helpers
{
    using System;

    /// <summary>
    /// Common class for converting <see cref="DateTime"/> between timezones.
    /// </summary>
    public static class DateTimeConverter
    {
        /// <summary>
        /// Convert user date to server date.
        /// </summary>
        /// <param name="date"><see cref="DateTime">User date</see>.</param>
        /// <param name="utc">User timezone.</param>
        /// <returns><see cref="DateTime">Server <see cref="DateTime"/></see>.</returns>
        public static DateTime ToServerDate(this DateTime date, int utc)
        {
            return date.AddHours(-utc).ToLocalTime();
        }

        /// <summary>
        /// Convert server date to user date.
        /// </summary>
        /// <param name="date"><see cref="DateTime">Server date</see>.</param>
        /// <param name="utc">User timezone.</param>
        /// <returns><see cref="DateTime">User <see cref="DateTime"/></see>.</returns>
        public static DateTime ToUserDate(this DateTime date, int utc)
        {
            return date.ToUniversalTime().AddHours(utc);
        }
    }
}
