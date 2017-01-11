using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public interface IClientStore<TClient> where TClient : IClient
    {
        Task<IEnumerable<TClient>> GetClientsAsync();

        Task<TClient> GetClientAsync(string clientId);

        Task<bool> CreateClientAsync(TClient client);

    }
}
