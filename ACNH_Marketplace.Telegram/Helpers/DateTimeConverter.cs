// <copyright file="DateTimeConverter.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Helpers
{
    using System;
    using System.Net;

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

        /// <summary>
        /// Check whether date time in the same week as other date.
        /// </summary>
        /// <param name="date1">First date.</param>
        /// <param name="date2">Second date (default to DateTime.Today).</param>
        /// <returns>Is dates week same.</returns>
        public static bool IsSameWeek(this DateTime date1, DateTime? date2 = null)
        {
            date2 = date2 ?? DateTime.Today;
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
            var d2 = date2.Value.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2.Value));
            return d1 == d2;
        }
    }
}
