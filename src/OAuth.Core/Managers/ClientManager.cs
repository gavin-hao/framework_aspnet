using Dongbo.OAuth.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public class ClientManager : ClientManager<IClient>
    {
        public ClientManager(IClientStore<IClient> store) : base(store) { }
    }
    public class ClientManager<TClient> : IClientManager where TClient : IClient
    {
        private IClientStore<TClient> ClientStore { get; set; }
        //public ICryptoProvider CryptoProvider { get; set; }
        public ClientManager(IClientStore<TClient> store)
        {
            this.ClientStore = store;
            //this.CryptoProvider = cryptoProvider;
        }
        /// <summary>
        ///  Authenticates the client. Used when authenticating with the authorization_code grant type.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> AuthenticateClientAsync(string clientId, string redirectUri)
        {
            var client = await this.ClientStore.GetClientAsync(clientId);

            if (client != null && client.Enabled && client.RedirectUri == redirectUri)
            {
                ClaimsIdentity identity = new ClaimsIdentity(Constants.AuthenticationType);
                identity.AddClaims(new List<Claim>() { 
                 new Claim(ClaimTypes.Name,clientId),
                 new Claim(Constants.ClaimType.RedirectUri,client.RedirectUri),
                  new Claim(ClaimTypes.AuthenticationMethod,Constants.AuthenticationMethod.ClientId),
                });


                if (identity.IsAuthenticated)
                {
                    // TODO: Update lastused date


                    return identity;
                }
            }
            //return Anonymous identyty
            return Anonymous;
        }
        private ClaimsIdentity Anonymous
        {
            get
            {
                return new ClaimsIdentity();
            }

        }
        /// <summary>
        ///  Authenticates the client. Used when authenticating with the client_credentials grant type.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> AuthenticateClientAsync(string clientId, IEnumerable<string> scope)
        {
            var client = await this.ClientStore.GetClientAsync(clientId);

            if (client != null && client.Enabled)
            {
                ClaimsIdentity identity = new ClaimsIdentity(Constants.AuthenticationType);
                identity.AddClaims(new List<Claim>() { 
                 new Claim(ClaimTypes.Name,clientId),
                 new Claim(Constants.ClaimType.RedirectUri,client.RedirectUri),
                  new Claim(ClaimTypes.AuthenticationMethod,Constants.AuthenticationMethod.ClientId),
                   new Claim(Constants.ClaimType.Scope,string.Join(" ", scope))
                });


                if (identity.IsAuthenticated)
                {
                    // TODO: Update lastused date

                    return identity;
                }
            }

            return Anonymous;
        }
        /// <summary>
        /// Authenticates the client using client id and secret.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> AuthenticateClientCredentialsAsync(string clientId, string clientSecret)
        {
            var client = await this.ClientStore.GetClientAsync(clientId);

            if (client != null && client.Enabled)
            {

                if (clientSecret == client.ClientSecret)
                {
                    ClaimsIdentity identity = new ClaimsIdentity(Constants.AuthenticationType);
                    identity.AddClaims(new List<Claim>() { 
                 new Claim(ClaimTypes.Name,clientId),
                 new Claim(Constants.ClaimType.RedirectUri,client.RedirectUri),
                  new Claim(ClaimTypes.AuthenticationMethod,Constants.AuthenticationMethod.ClientCredentials),
                });


                    if (identity.IsAuthenticated)
                    {
                        // TODO: Update lastused date

                        return identity;
                    }

                }
            }

            return Anonymous;
        }

    }
}
