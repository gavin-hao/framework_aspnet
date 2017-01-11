using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{
    public class RankingOptions
    {
        /// <summary>
        /// Filter TopN result.
        /// </summary>
        public long TopN { get; set; }

        /// <summary>
        /// Keep 
        /// This is not a client cache but a redis cache. Result of ZUNIONSTORE has an expiration of this cacheduration.
        /// </summary>
        public TimeSpan? CacheDuration { get; set; }

        public IWeightFunction weightFunc { get; set; }

        public RankingOptions(TimeSpan? cacheDuration = null, long topN = -1, IWeightFunction weightFunc = null)
        {
            this.CacheDuration = cacheDuration;
            this.TopN = topN;
            this.weightFunc = weightFunc != null ? weightFunc : WeightFunction.Empty;
        }

        /// <summary>
        /// Internal Default Constuctor.
        /// </summary>
        internal RankingOptions()
        {
            this.CacheDuration = TimeSpan.Zero;
            this.TopN = 25;
            this.weightFunc = WeightFunction.Empty;
        }
    }
}
