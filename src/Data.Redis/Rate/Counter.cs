using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{
    public interface ICounter
    {
        /// <summary>
        /// Add Hit(s) to multiple sources at the same time.
        /// This method should be called by clients when something just happened (page view, click, just played a game, ...)
        /// </summary>
        /// <param name="source">Sources of the hit</param>
        /// <param name="occurred">Date of the Hit (Utc).Null means just now.</param>
        /// <param name="hits">number of hits (default : 1)</param>
        /// <param name="dimension">context of the hit (default : 1)</param>
        /// <param name="localGranularities">Override global configuration granularities (default : null, aka Global Configured Granularities)</param>
        /// <returns>A task indicating success or failure.</returns>
        Task<bool> HitAsync(string[] eventSources, long hits = 1, string[] dimensions = null, DateTime? occurred = null, Granularity[] localGranularities = null);

        /// <summary>
        /// Used to flush one or more dimensions (remove everyhting concerning a dimension)
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        Task FlushDimensionsAsync(string[] dimensions = null);
    }

    public class Counter : ICounter
    {
        private readonly RedisClient redisConnection;
        private readonly IRateContext context;
        private readonly IDatabase DB;
        internal Counter(RedisClient redisConnection, IRateContext context)
        {
            if (redisConnection == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.redisConnection = redisConnection;
            this.context = context;
            this.DB = this.redisConnection.GetDatabase();
        }

        public async Task<bool> HitAsync(string[] eventSources, long hits = 1, string[] dimensions = null, DateTime? occurred = null, Granularity[] localGranularities = null)
        {
            if (eventSources == null || eventSources.Length == 0)
            {
                Trace.TraceWarning("HitAsync Failed because eventSources is null or empty.");
                return false;
            }
            if (eventSources.All(s => string.IsNullOrEmpty(s)))
            {
                Trace.TraceWarning("HitAsync Failed because all eventsources are null or empty.");
                return false;
            }
            if (occurred.HasValue && occurred.Value.Kind != System.DateTimeKind.Utc)
            {
                Trace.TraceWarning("HitAsync Failed because occured data is not Utc.");
                return false;
            }

            var success = await RedisTransactionProvider.Invoke(async (db) =>
            {

                var now = occurred ?? DateTime.UtcNow;
                dimensions = dimensions == null || dimensions.Length == 0 ? new string[] { Constants.DefaultDimension } : dimensions;
                await db.SetAddAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions), dimensions.Select(d => (RedisValue)d).ToArray());
                var granularities = localGranularities ?? this.context.Granularities;
                for (var g = 0; g < granularities.Length; g++)
                {
                    var granularity = granularities[g];
                    var ts = now.ToRoundedTimestamp(granularity.Factor);

                    for (var d = 0; d < dimensions.Length; d++)
                    {
                        var key = this.context.KeyFactory.NsKey(dimensions[d], granularity.Name, ts.ToString());

                        foreach (var eventSource in eventSources)
                        {
                            // increment sorted set
                            await db.SortedSetIncrementAsync(key, eventSource, hits);
                        }

                        // keys expiration (if occured date is not too far)
                        await db.KeyExpireAsync(key, (ts + granularity.Ttl).ToDateTime());
                    }
                }

            }, this.DB);
            return success;

        }





        public async Task FlushDimensionsAsync(string[] dimensions = null)
        {
            if (dimensions == null)
            {
                var values = await this.DB.SetMembersAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions));
                dimensions = values.Select(s => s.ToString()).ToArray();
            }
            var now = DateTime.UtcNow;
            //generate all keys to be deleted by 
            foreach (var granularity in this.context.Granularities)
            {
                var toInSeconds = now.ToRoundedTimestamp(granularity.Factor);
                var fromInSeconds = granularity.GetMinSecondsTimestamp(now);

                var allkeys = new List<RedisKey>();
                foreach (var ts in granularity.BuildFlatMap(fromInSeconds, toInSeconds))
                {
                    foreach (var context in dimensions)
                        allkeys.Add(this.context.KeyFactory.NsKey(context, granularity.Name, ts.ToString()));
                }

                await this.DB.KeyDeleteAsync(allkeys.ToArray(), CommandFlags.FireAndForget);

            }
            foreach (var dimension in dimensions)
                await this.DB.SetRemoveAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions), dimension);

        }
    }
}
