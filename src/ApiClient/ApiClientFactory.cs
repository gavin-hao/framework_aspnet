using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.ApiClient
{
    internal class ServiceConfigManager
    {
        private static readonly Dictionary<string, ServiceClient> _clientCache = new Dictionary<string, ServiceClient>();
        private ServiceClientSetting _conf = ServiceClientSetting.Instance;
        private static ServiceConfigManager instance = new ServiceConfigManager();
        private ServiceConfigManager()
        {
            ServiceClientSetting.ConfigChanged += new EventHandler(RESTClientSettings_ConfigChanged);
        }
        public static ServiceConfigManager Instance
        {
            get { return instance; }
        }
        private void RESTClientSettings_ConfigChanged(object sender, EventArgs e)
        {
            clearCache();
        }
        private void clearCache()
        {
            lock (_clientCache)
            {
                _clientCache.Clear();
            }

        }
        public bool ContainsKey(string key)
        {
            return _clientCache.ContainsKey(key);
        }
        public ServiceClient this[string key]
        {
            get
            {
                if (_clientCache.ContainsKey(key))
                    return _clientCache[key];

                lock (_clientCache)//if not exists ,then try lock and init it
                {
                    var clientConf = ServiceClientSetting.Instance[key];
                    if (clientConf == null)
                    {
                        throw new ArgumentNullException(string.Format("we cannot initialize a ServiceClient instance,cannot find the specific service config section [{0}]", key));
                        //return null;
                    }

                    ServiceClient client = null;
                    if (!ValidConfig(clientConf))
                    {
                        throw new NotSupportedException("we cannot initialize a ServiceClient instance ,the service config section is invalid !");
                        //return null;
                    }

                    client = new ServiceClient(clientConf);
                    //client.Timeout = (double)clientConf.Behavior.Timeout;

                    if (client != null)
                    {
                        _clientCache[key] = client;
                        return client;
                    }
                    else
                    {
                        return null;

                    }
                }
            }
        }
        private bool ValidConfig(Service ServerConfig)
        {

            return ServerConfig != null && !string.IsNullOrEmpty(ServerConfig.EndPoint) &&
                Uri.IsWellFormedUriString(ServerConfig.EndPoint, UriKind.Absolute) && ServerConfig.Enable;
        }
    }
    public class ApiClientFactory
    {

        private static readonly object _lock = new object();
        public static ServiceClient GetClient(string serviceName)
        {
            return ServiceConfigManager.Instance[serviceName];

        }

    }
}
