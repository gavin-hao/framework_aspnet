using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{
    /// <summary>
    /// 用于实现计数器管理，接口限速器，排行榜等功能
    /// a minimalist library to manage countings, rankings & overall leaderboard;
    /// 代码借鉴<see cref="https://github.com/Cybermaxs/Toppler"/>,原作者 @Cybermaxs ;
    /// 安装原项目<code>Install-Package Toppler</code> 
    /// </summary>
    /// <example>
    ///  Rate.Setup(redisConfiguration: "localhost:6379");
    /// Rate.Counter.HitAsync("myevent");
    /// var tops = await Rate.Ranking.AllAsync(Granularity.Day);
    /// </example>
    public class Rate
    {
        private static Lazy<RedisClient> lazyConnector;

        private static IRateContext options;

        public static bool IsConnected
        {
            get { return lazyConnector != null && lazyConnector.IsValueCreated && lazyConnector.Value.IsConnected; }
        }

        private static RedisClient Connection
        {
            get { return lazyConnector.Value; }
        }

        public static void Setup(string @namespace = Constants.DefaultNamespace, Granularity[] granularities = null)
        {
            if (options == null && lazyConnector == null)
            {
                options = new RateContext(@namespace, granularities ?? new Granularity[] { Granularity.Second, Granularity.Minute, Granularity.Hour, Granularity.Day });
                lazyConnector = new Lazy<RedisClient>(() => { return RedisClientFactory.Connect("cache"); });
            }
        }

        #region Counter Api
        private static Lazy<ICounter> counterInstance = new Lazy<ICounter>(CreateCounter, true);

        /// <summary>
        /// Default Entry point for Clients.
        /// </summary>
        public static ICounter Counter
        {
            get
            {
                return counterInstance.Value;
            }
        }

        internal static ICounter CreateCounter()
        {
            if (options == null)
                throw new InvalidOperationException("Setup hasn't been called yet.");

            return new Counter(Connection, options);
        }
        #endregion

        #region Ranking Api
        private static Lazy<IRanking> rankingInstance = new Lazy<IRanking>(CreateRankingApi, true);

        /// <summary>
        /// Default Entry point for Clients.
        /// Use it to get tops & rankings.
        /// </summary>
        public static IRanking Ranking
        {
            get
            {
                return rankingInstance.Value;
            }
        }

        internal static IRanking CreateRankingApi()
        {
            if (options == null)
                throw new InvalidOperationException("Setup hasn't been called yet.");

            return new Ranking(Connection, options);
        }
        #endregion
        #region Limiter Api
        private static Lazy<ILimiter> limitInstance = new Lazy<ILimiter>(CreateLimitApi, true);

        /// <summary>
        /// Default Entry point for Clients.
        /// Use it to get tops & rankings.
        /// </summary>
        public static ILimiter Limiter
        {
            get
            {
                return limitInstance.Value;
            }
        }

        internal static ILimiter CreateLimitApi()
        {
            if (options == null)
                throw new InvalidOperationException("Setup hasn't been called yet.");

            return new Limiter(Connection, options);
        }
        #endregion
    }
}
