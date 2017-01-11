using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Specialized;
using System.Collections.Concurrent;
namespace Dongbo.Redis
{

    public class RedisClient : IDisposable
    {
        private ConnectionMultiplexer connection = null;
        private string config = "";
        /// <summary>
        /// The redis connection
        /// </summary>
        /// <value>The redis connection.</value>
        public ConnectionMultiplexer RedisConnection
        {
            get
            {
                if (connection == null)
                    connection = ConnectionMultiplexer.Connect(config);
                return connection;
            }
        }
        public bool IsConnected
        {
            get { return this.RedisConnection != null && this.RedisConnection.IsConnected; }
        }
        public IDatabase GetDatabase(int db = -1)
        {
            return this.RedisConnection.GetDatabase(db);
        }

        /// <summary>
        /// The serializer
        /// </summary>
        /// <value>The serializer.</value>
        //public ISerializer Serializer { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisProviderContext"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="log">The textwriter to use for logging purposes.</param>
        public RedisClient(string configuration)
        {

            connection = ConnectionMultiplexer.Connect(configuration);
            //connection.ConnectionFailed += connection_ConnectionFailed;
            config = configuration;
            //Serializer = serializer;


        }

        void connection_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {

            this.Dispose();
        }

        public IDatabase Cache { get { return this.GetDatabase(-1); } }

        public ISubscriber PubSub { get { return this.RedisConnection.GetSubscriber(); } }
        public void Dispose()
        {
            if (RedisConnection != null)
            {
                RedisConnection.Dispose();
                connection = null;
            }
        }
    }
    public class RedisClientFactory
    {
        private static ConcurrentDictionary<string, RedisClient> pooledClientManager = new ConcurrentDictionary<string, RedisClient>();
        private static readonly object locker = new object();
        static RedisClientFactory()
        {
            RedisConfiguration.ConfigChanged += RedisConfiguration_ConfigChanged;
        }

        static void RedisConfiguration_ConfigChanged(object sender, EventArgs e)
        {
            lock (pooledClientManager)
            {
                //if (pooledClientManager != null)
                //{
                //    foreach (var client in pooledClientManager)
                //    {
                //        client.Value.Dispose();
                //    }

                //}
                pooledClientManager.Clear();
            }

        }
        public static RedisClient Connect(string clusterName = "cache")
        {
            RedisClient client = null;
            if (!pooledClientManager.TryGetValue(clusterName, out client))
            {
                var conf = RedisConfiguration.Instance[clusterName];
                if (conf == null && RedisConfiguration.Instance.Clusters != null)
                    conf = RedisConfiguration.Instance.Clusters.FirstOrDefault();
                if (conf != null)
                {
                    client = new RedisClient(conf.ToString());
                }
                else
                {
                    client = new RedisClient("localhost:6379");
                }
                pooledClientManager.TryAdd(clusterName, client);
            }

            return client;
        }


    }





}
