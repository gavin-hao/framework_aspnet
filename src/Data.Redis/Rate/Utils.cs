using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{
    internal static class DateTimeExtensions
    {
        public const long EpochTicks = 621355968000000000;
        public const long TicksPeriod = 10000000;
        public const long TicksPeriodMs = 10000;

        //epoch time
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        /// <summary>
        /// Number of milliseconds since epoch(1/1/1970).
        /// </summary>
        public static long ToTimestamp(this DateTime date)
        {
            long ts = (date.Ticks - EpochTicks) / TicksPeriodMs;
            return ts;
        }

        /// <summary>
        /// Number of seconds since epoch(1/1/1970).
        /// </summary>
        public static long ToSecondsTimestamp(this DateTime date)
        {
            long ts = (date.Ticks - EpochTicks) / TicksPeriod;
            return ts;
        }

        /// <summary>
        /// Round a timestamp in seconds to th floor step.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static long ToRoundedTimestamp(this DateTime date, int precision)
        {
            return ((long)date.ToSecondsTimestamp() / precision) * precision;
        }
    }
    internal static class Int64Extensions
    {
        public const long EpochTicks = 621355968000000000;
        public const long TicksPeriod = 10000000;
        public const long TicksPeriodMs = 10000;

        /// <summary>
        /// Round a timestamp in seconds to the desired precision.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static long ToRoundedTimestamp(this Int64 timestamp, long precision)
        {
            return ((long)timestamp / precision) * precision;
        }

        /// <summary>
        /// Convert a timestamp to DateTime.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this Int64 timestamp)
        {
            return new DateTime(TimeSpan.FromSeconds(timestamp).Ticks + EpochTicks, DateTimeKind.Utc);
        }
    }
    internal static class StringExtensions
    {
        private static Regex alphaRegex = new Regex("^[a-zA-Z0-9._]*$", RegexOptions.Compiled | RegexOptions.Singleline);
        private const string separator = ":";

        /// <summary>
        /// Check if string is ONLY alphanumeric (regex ^[a-zA-Z0-9]*$)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool AlphaNumericString(this string s)
        {
            return alphaRegex.IsMatch(s);
        }

        /// <summary>
        /// Generate A Redis Key using ':' (semi colon) as delimiter.
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string AsRedisKey(this string[] parts)
        {
            return string.Join(separator, parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
    internal static class ArgumentCheckExtensions
    {
        public static void ThrowIfNull<T>(this T obj, string name = "") where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(name);
        }
        public static void ThrowIfDefault<T>(this T val, string name = "") where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(val, default(T)))
                throw new ArgumentException(name);
        }
    }


    internal class RedisTransactionProvider
    {
        public static async Task<bool> Invoke(Func<IDatabaseAsync,Task> commands, IDatabase db)
        {

            try
            {
                var redisDb = db;

                var tran = redisDb.CreateTransaction();
                //invoke
                await commands(tran);
                return await tran.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Trace.TraceError("failed to invoke batch. Reason :" + ex.Message);
                return false;
            }
        }

    }
}
