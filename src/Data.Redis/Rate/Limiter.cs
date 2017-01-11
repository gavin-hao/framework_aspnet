using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{
    /// <summary>
    /// 限速器
    /// </summary>
    public interface ILimiter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventSource">key of an limiter ,{[ip|userId]:ns:api_path}</param>
        /// <param name="ttl">interval of seconds</param>
        /// <param name="limit">rate</param>
        /// <param name="waitMilliseconds"> min interval in millseconds between two calls </param>
        /// <returns></returns>
        Task<bool> ShouldThrottleAsync(string eventSource, int ttl, int limit, long waitMilliseconds);
    }
    /// <summary>
    /// 通过redis ->sorted set 实现，类似 token buckets 限速器
    /// algorithm:
    /// 1. each user has a sorted set ,with keys and values {identical,current call api time(ms)},
    /// 2. When a user attempts to perform an action, we first drop all elements of the set which occured before one interval ago. This can be accomplished with Redis’s ZREMRANGEBYSCORE command.
    /// 3. We fetch all elements of the set, using ZRANGE(0, -1).
    /// 4. We add the current timestamp to the set, using ZADD.
    /// 5. We set a TTL equal to the rate-limiting interval on the set (to save space)
    /// 6. if(count the number of fetched elements >limit) then exceeded ->don't allow 
    /// 7. We also can compare the largest fetched element to the current timestamp. If they’re too close, we also don’t allow the action.
    /// </summary>
    public class Limiter : ILimiter
    {
        private readonly RedisClient redisConnection;
        private readonly IRateContext context;
        private readonly IDatabase DB;
        private readonly int Interval;
        internal Limiter(RedisClient redisConnection, IRateContext context)
        {
            if (redisConnection == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.redisConnection = redisConnection;
            this.context = context;
            this.DB = this.redisConnection.GetDatabase();
            this.Interval = Granularity.Second.Ttl;
        }
        public async Task<bool> ShouldThrottleAsync(string eventSource, int ttl, int limit, long minIntervalInTTL = 0)
        {
            if (string.IsNullOrEmpty(eventSource))
            {
                Trace.TraceWarning("HitAsync Failed because eventSource is null or empty.");
                return false;
            }
            if (ttl < 1)
            {
                Trace.TraceWarning("HitAsync Failed , Must pass a positive integer for ttl.");
                return false;
            }
            if (limit < 1)
            {
                Trace.TraceWarning("HitAsync Failed , Must pass a positive integer for limit.");
                return false;
            }
            if (minIntervalInTTL <= 0)
                minIntervalInTTL = 0;
            var throttle = await Attempt(eventSource, ttl, limit, minIntervalInTTL);
            if (throttle > 0)
            {
                //exceeded 
                return true;
            }
            return false;
        }

        private async Task<int> Attempt(string eventSource, int ttl, int limit, long minIntervalInTTL)
        {
            //minIntervalInTTL = ttl / limit;
            var now = DateTime.UtcNow.ToTimestamp();
            var key = this.context.KeyFactory.NsKey(eventSource);
            var clearBefore = now - ttl * 1000;


            var muti = this.DB.CreateTransaction();
            await muti.SortedSetRemoveRangeByScoreAsync(key, 0, clearBefore);
            var allRange = await muti.SortedSetRangeByRankAsync(key);
            await muti.SortedSetAddAsync(key, now, now);
            await muti.KeyExpireAsync(key, TimeSpan.FromSeconds(ttl));
            var result = await muti.ExecuteAsync();

            if (result)
            {
                var toMany = allRange.Length >= limit;
                var lastCallTime = allRange.Last();
                var diff = minIntervalInTTL > 0 ? now - (long)lastCallTime : 0L;
                if (toMany || diff < minIntervalInTTL)
                {
                    //var nextTick = (int)Math.Min((long)allRange[0] - now + ttl * 1000, minIntervalInTTL > 0 ? minIntervalInTTL - diff : long.MaxValue) / 1000;
                    //return nextTick;
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
    }
}
