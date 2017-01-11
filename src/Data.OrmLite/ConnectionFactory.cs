using Dongbo.Configuration;
using Dongbo.Logging;
using ServiceStack.OrmLite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OrmLite
{

    public static class ConnectionFactory
    {
        private static readonly LogWrapper log = new LogWrapper();
        public static int TotalOpenedConnection = 0;
        public static int DisposeCount = 0;
        //private static List<IDbConnection> DbConns = new List<IDbConnection>();
        //private static ConcurrentDictionary<string, OrmLiteConnectionFactory> ConnFactorys = new ConcurrentDictionary<string, OrmLiteConnectionFactory>();
        static ConnectionFactory()
        {
            OrmLiteConnectionManager.RegisterConnectionDisposedNotification(StatusCollector);
        }

        private static void StatusCollector(object sender, EventArgs e)
        {
            DisposeCount++;
            TotalOpenedConnection--;
        }
        /// <summary>
        /// get a DbConnection Instance
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public static System.Data.IDbConnection GetConn(string database)
        {


            OrmLiteConnectionFactory factory = OrmLiteConnectionManager.Instance[database];
            if (factory == null)
                throw new ArgumentException(string.Format("cannt get connection for database:{0}", database));

            var conn = factory.Open();

            TotalOpenedConnection++;
            if (TotalOpenedConnection > 100)
            {
                log.Warn(string.Format("Current opend DbConnection count >={0}", TotalOpenedConnection));
            }
            return conn;
        }
    }

    internal class OrmLiteConnectionManager
    {
        private static ConcurrentDictionary<string, OrmLiteConnectionFactory> ConnFactorys = new ConcurrentDictionary<string, OrmLiteConnectionFactory>();
        private OrmLiteConnectionManager()
        {
            ConnectionStringCollection.RegisterConfigChangedNotification(OnConfigChanged);
        }

        private static OrmLiteConnectionManager instance = new OrmLiteConnectionManager();

        public static OrmLiteConnectionManager Instance { get { return instance; } }

        void OnConfigChanged(object sender, EventArgs args)
        {
            ConnFactorys.Clear();
            //ConnectionStringCollection collection = (ConnectionStringCollection)sender;
            //foreach (string instanceName in collection)
            //{

            //    string connString = collection[instanceName];
            //    if (ConnFactorys.ContainsKey(instanceName))
            //    {
            //        var db = ConnFactorys[instanceName];
            //        db.ConnectionString = connString;
            //    }
            //}
        }
        static EventHandler _connectionDisposehandler;

        public static void RegisterConnectionDisposedNotification(EventHandler handler)
        {
            _connectionDisposehandler += handler;
        }
        internal OrmLiteConnectionFactory this[string database]
        {
            get
            {
                OrmLiteConnectionFactory factory;
                if (!ConnFactorys.TryGetValue(database, out factory))
                {
                    var connectStr = ConnectionStringProvider.GetConnectionString(database);
                    if (string.IsNullOrEmpty(connectStr)) { connectStr = database; }
                    factory = new OrmLiteConnectionFactory(connectStr, MySqlDialect.Provider);
                    factory.AutoDisposeConnection = true;
                    factory.OnDispose = m =>
                    {
                        if (_connectionDisposehandler != null)
                        {
                            _connectionDisposehandler(factory, EventArgs.Empty);
                        }
                    };
                    ConnFactorys.TryAdd(database, factory);
                }
                return factory;
            }

        }
        internal bool Contains(string name)
        {
            return ConnFactorys.ContainsKey(name);
        }


    }


}
