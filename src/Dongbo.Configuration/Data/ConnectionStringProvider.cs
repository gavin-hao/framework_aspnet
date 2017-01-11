using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Dongbo.Configuration
{
    public class ConnectionStringProvider
    {
        public static string GetConnectionString(string key)
        {
            return ConnectionStringCollection.Instance[key];
        }

        public static ConnectionStringEntry GetConnectionConfiguration(string key)
        {
            ConnectionStringEntry entry = null;
            foreach (var item in ConnectionStringCollection.Instance.Entries)
            {
                if (item.Name == key)
                {
                    entry = item;
                    break;
                }

            }
            return entry;
        }
    }
}
