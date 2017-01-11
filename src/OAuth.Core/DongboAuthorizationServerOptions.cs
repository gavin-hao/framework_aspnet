using Dongbo.OAuth.Core.Providers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public class DongboAuthorizationServerOptions
    {
        private DongboAuthorizationServerEvents events;
        public DongboAuthorizationServerEvents Events
        {
            get
            {
                return this.events ?? (this.events = new DongboAuthorizationServerEvents());
            }
        }

        public DongboAuthorizationServerOptions()
        {
            this.AccessTokenLifetime = TimeSpan.FromDays(1);
            this.AuthorizationCodeLifetime = TimeSpan.FromMinutes(5);
            this.RefreshTokenLifetime = TimeSpan.FromDays(90);
            this.AuthorizationCodeEndpointUrl = "/oauth/authorize";
            this.TokenEndpointUrl = "/oauth/token";
            this.IdentityEndpointUrl = "/openid/identity";
            TokenProvider = new TokenProvider(this);
        }

        /// <summary>Gets or sets the access token lifetime.</summary>
        /// <value>The access token lifetime.</value>
        public TimeSpan AccessTokenLifetime { get; set; }

        /// <summary>Gets or sets the authorization code lifetime.</summary>
        /// <value>The authorization code lifetime.</value>
        public TimeSpan AuthorizationCodeLifetime { get; set; }

        /// <summary>Gets or sets the refresh token lifetime.</summary>
        /// <value>The refresh token lifetime.</value>
        public TimeSpan RefreshTokenLifetime { get; set; }

        /// <summary>Gets or sets URI of the issuer.</summary>
        /// <value>The issuer URI.</value>
        public Uri IssuerUri { get; set; }

        /// <summary>
        /// Gets or sets the client management provider. This is the class responsible for locating and
        /// validating clients.
        /// </summary>
        /// <value>The client management provider.</value>
        public IClientManager ClientManager { get; set; }


        public Dongbo.Logging.LogWrapper Logger { get { return new Dongbo.Logging.LogWrapper(); } }

        /// <summary>
        /// Gets or sets the token store. This is the class responsible for creating and validating
        /// tokens and authorization codes.
        /// </summary>
        /// <value>The token store.</value>
        public ITokenManager TokenManager { get; set; }


        /// <summary>
        /// aspnet.Identity usermanager
        /// </summary>
        public IUserManager UserManager { get; set; }

        public ITokenProvider TokenProvider { get; set; }


        private ConcurrentDictionary<string, object> properties = new ConcurrentDictionary<string, object>();


        protected void Set<T>(string key, T value)
        {
            properties[key] = value;
        }

        protected T Get<T>(string key)
        {
            object value;
            return properties.TryGetValue(key, out value) ? (T)value : default(T);
        }
        private const string TokenManagerKey = "TokenManager_Value";
        private const string UserManagerKey = "UserManager_Value";
        private const string TokenProviderKey = "TokenProvider_value";

        //public ITokenManager<TAccessToken, TRefreshToken, TAuthorizationCode> Create<TAccessToken, TRefreshToken, TAuthorizationCode>
        //    (Func<ITokenStore, ITokenProvider<TAccessToken, TRefreshToken, TAuthorizationCode>, ILog, ITokenManager<TAccessToken, TRefreshToken, TAuthorizationCode>> createDelegate
        //    , ITokenStore store, ITokenProvider<TAccessToken, TRefreshToken, TAuthorizationCode> tokenProvider, ILog logger)
        //    where TAccessToken : IAccessToken
        //    where TRefreshToken : IRefreshToken
        //    where TAuthorizationCode : IAuthorizationCode
        //{
        //    if (createDelegate == null)
        //        throw new ArgumentNullException("createDelegate");

        //    var manager = createDelegate(store, tokenProvider, logger);
        //    Set<ITokenManager<TAccessToken, TRefreshToken, TAuthorizationCode>>(TokenManagerKey, manager);
        //    return manager;
        //}

        public TManager GetTokenManager<TManager>()
        {
            return Get<TManager>(TokenManagerKey);
        }



        //token provider
        public void SetTokenProviderr<TProvider>(TProvider manager)
        {
            Set<TProvider>(TokenProviderKey, manager);
        }
        public TProvider GetTokenProvider<TProvider>()
        {
            return Get<TProvider>(TokenProviderKey);
        }

        public string AuthorizationCodeEndpointUrl { get; set; }

        /// <summary>Gets or sets URL of the token endpoint.</summary>
        /// <value>The token endpoint URL.</value>
        public string TokenEndpointUrl { get; set; }

        /// <summary>Gets or sets URL of the identity endpoint.</summary>
        /// <value>The identity endpoint URL.</value>
        public string IdentityEndpointUrl { get; set; }
    }

    public class DongboAuthorizationServerEvents
    {
        /// <summary>
        /// Activated when the token has been issued.
        /// Use this event to do any special handling after the user has authenticated.
        /// </summary>
        /// <example>Set an authentication cookie to log in with token and cookie at the same time.</example>
        public Func<TokenIssuedEventArgs, Task> TokenIssued;

        /// <summary>
        /// Activated when the user is logged in (either via a username and password, or a refresh token)
        /// and the principal is created. Use this event to add any custom claims to the user before the
        /// token is created.
        /// </summary>
        public Func<IdentityCreatedEventArgs, Task> IdentityCreated;

        /// <summary>
        /// Activated when the token endpoint receives a request for authorization with a non-standard grant_type.
        /// </summary>
        /// <example>Handle application password grant types for applications that doesnt have a GUI.</example>
        public Func<UnknownGrantTypeReceivedEventArgs, Task> UnknownGrantTypeReceived;
    }

    public class UnknownGrantTypeReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the UnknownGrantTypeReceivedEventArgs class.
        /// </summary>
        /// <param name="context">The OAuth context, <see cref="Microsoft.Owin.Security.OAuth.OAuthGrantCustomExtensionContext"/>.</param>
        public UnknownGrantTypeReceivedEventArgs(object context)
        {
            this.Context = context;
        }

        /// <summary>Gets the OAuth context.</summary>
        /// <remarks><see cref="Microsoft.Owin.Security.OAuth.OAuthGrantCustomExtensionContext"/></remarks>
        /// <value>The OAuth context.</value>
        public object Context { get; private set; }
    }

    public class TokenIssuedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the TokenIssuedEventArgs class.
        /// </summary>
        /// <param name="context">The OAuth context, <see cref="Microsoft.Owin.Security.OAuth.OAuthTokenEndpointResponseContext"/>.</param>
        public TokenIssuedEventArgs(object context)
        {
            this.Context = context;
        }

        /// <summary>Gets the OAuth context.</summary>
        /// <remarks><see cref="Microsoft.Owin.Security.OAuth.OAuthTokenEndpointResponseContext"/></remarks>
        /// <value>The OAuth context.</value>
        public object Context { get; private set; }
    }

    public class IdentityCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the IdentityCreatedEventArgs class.
        /// </summary>
        /// <param name="principal">The identity.</param>
        /// <param name="context">The OAuth context, <see cref="Microsoft.Owin.Security.OAuth.OAuthGrantResourceOwnerCredentialsContext"/></param>
        public IdentityCreatedEventArgs(IIdentity identity, object context)
        {
            this.Identity = new ClaimsIdentity(identity);
            this.Context = context;
        }

        /// <summary>Gets the Identity.</summary>
        /// <value>The Identity.</value>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>Gets the OAuth context.</summary>
        /// <remarks><see cref="Microsoft.Owin.Security.OAuth.OAuthGrantResourceOwnerCredentialsContext"/></remarks>
        /// <value>The OAuth context.</value>
        public object Context { get; private set; }
    }
}
