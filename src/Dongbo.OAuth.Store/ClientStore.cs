using Dongbo.ApiClient;
using Dongbo.Data;
using Dongbo.OAuth.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Store
{
    public class ClientStore : IClientStore<IClient>
    {

        //public ServiceClient Service { get; private set; }
        public ClientStore()
        {
            
        }
        public async Task<IEnumerable<IClient>> GetClientsAsync()
        {
            var  Service = StoreConstants.OAuthClientServiceClient;
            var uri = "api/oauth/client/list";
            return await Service.GetAsync<List<OAuthClient>>(uri);
        }

        public async Task<IClient> GetClientAsync(string clientId)
        {
            var Service = StoreConstants.OAuthClientServiceClient;
            var uri = string.Format("api/oauth/client?clientId={0}", clientId);
            return await Service.GetAsync<OAuthClient>(uri);
        }

        public async Task<bool> CreateClientAsync(IClient client)
        {
            var Service = StoreConstants.OAuthClientServiceClient;
            if (client == null)
                throw new ArgumentNullException("client");
            var newclient = new OAuthClient(client);
            newclient.CreateTime = DateTime.Now;
            newclient.LastUsed = DateTime.Now;
            newclient.Enabled = true;//todo: valid enabled
           
            var uri = string.Format("api/oauth/client/create");
            return await Service.PostAsync<bool, OAuthClient>(uri, newclient);
        }

    }
}
