using System;

namespace EventPlusWeb1.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime AhoraEnColombia()
        {
            TimeZoneInfo zonaColombia = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaColombia);
        }
    }
}