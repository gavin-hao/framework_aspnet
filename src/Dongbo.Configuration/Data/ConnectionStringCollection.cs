using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

namespace Dongbo.Configuration
{
    public class ConnectionStringEntry
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("connectionString")]
        public string ConnectionString;
        
        [XmlAttribute("providerName")]
        public string ProviderName;
    }

    [XmlRoot(ConnectionStringCollection.SectionName)]
    public class ConnectionStringCollection : IPostSerializer
    {
        private const string SectionName = "connectionStrings";
        private static ConnectionStringCollection instance;

        static ConnectionStringCollection()
        {
            instance = RemoteConfigurationManager.Instance.GetSection<ConnectionStringCollection>(SectionName);
        }

        static EventHandler _handler;

        public static void RegisterConfigChangedNotification(EventHandler handler)
        {
            _handler += handler;
        }

        public static ConnectionStringCollection Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
                if (_handler != null)
                    _handler(value, EventArgs.Empty);
            }
        }


        [XmlElement]
        public bool EnabledDatabaseConnectivityState = false;

        [XmlElement("add")]
        public ConnectionStringEntry[] Entries;

        private NameValueCollection collection = new NameValueCollection();

        [XmlIgnore]
        public string this[string name]
        {
            get
            {
                return collection[name];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return collection.GetEnumerator();
        }


        #region IPostSerializer 成员

        /// <summary>
        /// combine local connectionString and remote connectionString together, local connection string will be the final if the name is conflict
        /// </summary>
        public void PostSerializer()
        {
            if (Entries != null)
            {
                foreach (ConnectionStringEntry entry in Entries)
                {
                    collection[entry.Name]= entry.ConnectionString;
                }
            }

            foreach (ConnectionStringSettings entry in ConfigurationManager.ConnectionStrings)
            {
                collection[entry.Name] = entry.ConnectionString;
            }
        }

        #endregion
    }
}
