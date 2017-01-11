﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{

    public interface IRanking
    {
        /// <summary>
        /// Get Tops in one or more dimensions.
        /// Extensions methods are avaible for common use cases.
        /// </summary>
        /// <param name="granularity">Granularity (Second, Minute, Hour, Day)</param>
        /// <param name="range">Number of points (0 means current, 1 means last two granularities, ...)</param>
        /// <param name="from">From this date.</param>
        /// <param name="dimensions">Requested Dimension(s). Null means all.</param>
        /// <param name="options">Ranking options (Cache, TopN,WeightFunction, ...) </param>
        /// <returns>List of TopResult</returns>
        Task<IEnumerable<RateResult>> AllAsync(Granularity granularity, int range = 0, DateTime? from = null, string[] dimensions = null, RankingOptions options = null);

        /// <summary>
        /// Get Details for a single event Source.
        /// Note : aggregation is computed but a single element is returned.
        /// </summary>
        /// <param name="eventSource">Requested EventSource.</param>
        /// <param name="granularity">Granularity (Second, Minute, Hour, Day)</param>
        /// <param name="range">Number of points (0 means current, 1 means last two granularities, ...)</param>
        /// <param name="from">From this date.</param>
        /// <param name="dimensions">Requested Dimension(s). Null means all.</param>
        /// <param name="options">Ranking options (Cache, TopN,WeightFunction, ...) </param>
        /// <returns>Requeted EventSource. Empty if not found.</returns>
        Task<RateResult> DetailsAsync(string eventSource, Granularity granularity, int range = 0, DateTime? from = null, string[] dimensions = null, RankingOptions options = null);
    }
    public class Ranking : IRanking
    {
        private readonly RedisClient redisConnection;
        private readonly IRateContext context;
        private readonly IDatabaseAsync DB;

        internal Ranking(RedisClient redisConnection, IRateContext context)
        {
            if (redisConnection == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.redisConnection = redisConnection;
            this.context = context;
            this.DB = this.redisConnection.GetDatabase();
        }

        public async Task<IEnumerable<RateResult>> AllAsync(Granularity granularity, int range = 0, DateTime? from = null, string[] dimensions = null, RankingOptions options = null)
        {
            options = options ?? new RankingOptions();
            var cacheKey = await ComputeAggregationAsync(granularity, range, from, dimensions, options).ConfigureAwait(false);

            var entries = await this.DB.SortedSetRangeByRankWithScoresAsync(cacheKey, 0, options.TopN, Order.Descending).ConfigureAwait(false);
            return entries.Select((e, i) =>
            {
                return new RateResult(e.Element, e.Score, i + 1);
            });
        }

        public async Task<RateResult> DetailsAsync(string eventSource, Granularity granularity, int range = 0, DateTime? from = null, string[] dimensions = null, RankingOptions options = null)
        {
            options = options ?? new RankingOptions();
            var cacheKey = await ComputeAggregationAsync(granularity, range, from, dimensions, options).ConfigureAwait(false);

            var rank = await DB.SortedSetRankAsync(cacheKey, eventSource, Order.Descending).ConfigureAwait(false);
            var score = await DB.SortedSetScoreAsync(cacheKey, eventSource).ConfigureAwait(false);

            return new RateResult(eventSource, score, rank + 1);
        }

        #region Private methods
        private async Task<string> ComputeAggregationAsync(Granularity granularity, int range = 0, DateTime? from = null, string[] dimensions = null, RankingOptions options = null)
        {
            var allkeys = await this.GetKeysAsync(this.DB, granularity, range, from, dimensions).ConfigureAwait(false);

            var allweights = new List<double>();
            for (var k = 0; k < allkeys.Length; k++)
            {
                allweights.Add(options.weightFunc.Weight(k, allkeys.Count()));
            }

            var cacheKey = this.context.KeyFactory.NsKey(dimensions != null ? this.context.KeyFactory.RawKey(dimensions) : Constants.SetAllDimensions, Constants.CacheKeyPart, options.weightFunc.Name, granularity.Name, range.ToString());

            if (options.CacheDuration.HasValue && options.CacheDuration.Value != TimeSpan.Zero)
            {
                //use cache
                bool exists = await this.DB.KeyExistsAsync(cacheKey).ConfigureAwait(false);
                if (!exists)
                {
                    await this.DB.SortedSetCombineAndStoreAsync(SetOperation.Union, cacheKey, allkeys).ConfigureAwait(false);
                    await this.DB.KeyExpireAsync(cacheKey, DateTime.UtcNow.Add(options.CacheDuration.Value), CommandFlags.FireAndForget).ConfigureAwait(false);
                }
            }
            else
                await this.DB.SortedSetCombineAndStoreAsync(SetOperation.Union, cacheKey, allkeys, allweights.ToArray()).ConfigureAwait(false);

            return cacheKey;
        }

        private async Task<RedisKey[]> GetKeysAsync(IDatabaseAsync db, Granularity granularity, int range, DateTime? from, string[] dimensions = null)
        {
            if (dimensions == null || dimensions.Length == 0)
            {
                // dimensions is null => load all registered dimensions from DB
                var values = await db.SetMembersAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions)).ConfigureAwait(false);
                dimensions = values.Select(s => s.ToString()).ToArray();
            }

            var allkeys = new List<RedisKey>();
            foreach (var ts in granularity.BuildFlatMap(from, range))
            {
                for (var d = 0; d < dimensions.Length; d++)
                {
                    allkeys.Add(this.context.KeyFactory.NsKey(dimensions[d], granularity.Name, ts.ToString()));
                }
            }

            return allkeys.ToArray();
        }
        #endregion
    }
}
