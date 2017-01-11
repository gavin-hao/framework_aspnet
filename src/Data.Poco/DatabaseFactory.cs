using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Dongbo.Configuration;
namespace Dongbo.Data.Poco
{
    public class DatabaseFactory
    {
        public static Database GetDatabase(string connectionStringName)
        {
            var config = GetConnectionString(connectionStringName);
            if (config == null || string.IsNullOrWhiteSpace(config.ConnectionString))
            {
                throw new ArgumentException("Unable to retrieve database " + connectionStringName + " from connection string configuration file.", connectionStringName);
            }
            var provider = string.IsNullOrWhiteSpace(config.ProviderName) ? "MySql.Data.MySqlClient" : config.ProviderName;
            return new Database(config.ConnectionString, provider);
        }
        public static Database GetDatabase(string connectionString, string providerName)
        {

            return new Database(connectionString, providerName);
        }
        private static ConnectionStringEntry GetConnectionString(string connectionStringName)
        {

            var entry = ConnectionStringProvider.GetConnectionConfiguration(connectionStringName);
            return entry;
        }
    }
}
