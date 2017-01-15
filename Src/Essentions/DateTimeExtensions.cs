using System;
using System.Globalization;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="DateTime" />. </summary>
    public static class DateTimeExtensions
    {
        private const string Rfc822Format = "ddd, dd MMM yyyy HH:mm:ss";

        private static readonly DateTime _UnixEpochStart = new DateTime(1970, 1, 1);

        /// <summary> Gets the RFC822 string representation of a date. </summary>
        /// <param name="date">The date.</param>
        /// <param name="withHourOffset">if set to <c>true</c>, will add hour offset at the end.</param>
        /// <returns>RFC822 string representation of a date.</returns>
        public static string ToRfc822(this DateTime date, bool withHourOffset = true)
        {
            var utcDate = date.ToUniversalTime();
            var result = utcDate.ToString(Rfc822Format, CultureInfo.InvariantCulture);

            if (withHourOffset) {
                var hourDiff = date.Hour - utcDate.Hour;
                var hourOffset = hourDiff.ToString("00", CultureInfo.InvariantCulture) + "00";
                if (hourDiff >= 0) hourOffset = "+" + hourOffset;

                result += hourOffset;
            }

            return result;
        }

        /// <summary> Gets the unix epoch time of the date. </summary>
        /// <returns> Total seconds from 1 Jan 1970. </returns>
        public static long ToUnixTime(this DateTime date)
        {
            return Convert.ToInt64((date - _UnixEpochStart).TotalSeconds);
        }
    }
}