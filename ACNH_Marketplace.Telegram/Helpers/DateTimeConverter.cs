using System;
using System.Collections.Generic;
using System.Text;

namespace ACNH_Marketplace.Telegram.Helpers
{
    public static class DateTimeConverter
    {
        public static DateTime ToServerDate(this DateTime date, int utc)
        {
            return date.AddHours(-utc).ToLocalTime();
        }
        public static DateTime ToUserDate(this DateTime date, int utc)
        {
            return date.ToUniversalTime().AddHours(utc);
        }
    }
}
