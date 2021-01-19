using System;
using System.Globalization;

namespace Shared.Common.Extension
{
    public static class Extensions
    {
        public static decimal RoundNumber(this decimal value, int round = 100)
        {
            if (value <= 0) return 0;

            return Math.Round(value / round, 0) * round;
        }

        public static int AsId(this object item, int defaultId = -1)
        {
            if (item == null)
                return defaultId;

            int result;
            if (!int.TryParse(item.ToString(), out result))
                return defaultId;

            return result;
        }

        public static int AsInt(this object item, int defaultInt = default)
        {
            if (item == null)
                return defaultInt;

            int result;
            if (!int.TryParse(item.ToString(), out result))
                return defaultInt;

            return result;
        }

        public static int AsEnumToInt(this object item, int defaultInt = default)
        {
            if (item == null)
                return defaultInt;
            return (int) item;
        }

        public static long AsEnumToLong(this object item, long defaultInt = default)
        {
            if (item == null)
                return defaultInt;
            return (long) item;
        }

        public static long AsLong(this object item, long defaultInt = default)
        {
            if (item == null)
                return defaultInt;

            long result;
            if (!long.TryParse(item.ToString(), out result))
                return defaultInt;

            return result;
        }

        public static double AsDouble(this object item, double defaultDouble = default)
        {
            if (item == null)
                return defaultDouble;

            double result;
            if (!double.TryParse(item.ToString(), out result))
                return defaultDouble;

            return result;
        }

        public static decimal AsDecimal(this object item, decimal defaultDecimal = default)
        {
            if (item == null)
                return defaultDecimal;

            decimal result;
            if (!decimal.TryParse(item.ToString(), out result))
                return defaultDecimal;

            return result;
        }

        public static short AsShort(this object item, short defaultShort = default)
        {
            if (item == null)
                return defaultShort;

            short result;
            if (!short.TryParse(item.ToString(), out result))
                return defaultShort;

            return result;
        }

        public static byte AsByte(this object item, byte defaultByte = default)
        {
            if (item == null)
                return defaultByte;

            byte result;
            if (!byte.TryParse(item.ToString(), out result))
                return defaultByte;

            return result;
        }

        public static string AsDateIsoString(this DateTime item)
        {
            return item.ToString("o");
        }

        public static DateTime GetCurrentDateUtc()
        {
            return DateTime.UtcNow;
        }

        public static DateTime AsDateTime(this object item, DateTime defaultDateTime = default)
        {
            if (item == null || string.IsNullOrEmpty(item.ToString()))
                return defaultDateTime;

            DateTime result;
            if (!DateTime.TryParse(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", item), out result))
                return defaultDateTime;

            return result;
        }

        public static DateTime AsDateTime(this string item, string fomat, DateTime defaultDateTime = default)
        {
            if (item == null || string.IsNullOrEmpty(item))
                return defaultDateTime;
            try
            {
                var result = DateTime.ParseExact(item, fomat, CultureInfo.InvariantCulture);
                return result;
            }
            catch (Exception)
            {
                return defaultDateTime;
            }
        }

        public static string ToUnsign(this string input)
        {
            return Utility.ToUnsign(input);
        }

        public static string ToFriendlyUrl(this string input)
        {
            return Utility.ToFriendlyUrl(input);
        }

        #region ToEnum<T>

        public static T AsEnum<T>(this object input, T _default)
        {
            return Utility.ToEnum(input, _default);
        }

        public static T AsEnumOrDefault<T>(this object input)
        {
            return Utility.ToEnumOrDefault<T>(input);
        }

        #endregion ToEnum<T>
    }
}