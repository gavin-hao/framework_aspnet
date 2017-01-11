using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.OAuthTokenAuthenticationMiddleware
{
   
    public class OAuthTokenAuthenticationMiddleware : AuthenticationMiddleware<OAuthTokenAuthenticationOptions>
    {

        private readonly string _challenge;

        /// <summary>
        /// Bearer authentication component which is added to an OWIN pipeline. This constructor is not
        /// called by application code directly, instead it is added by calling the the IAppBuilder UseOAuthBearerAuthentication 
        /// extension method.
        /// </summary>
        public OAuthTokenAuthenticationMiddleware(
            OwinMiddleware next,
            IAppBuilder app,
            OAuthTokenAuthenticationOptions options)
            : base(next, options)
        {

            if (!string.IsNullOrWhiteSpace(Options.Challenge))
            {
                _challenge = Options.Challenge;
            }
            else
            {
                _challenge = "token";
            }
           
            //if (Options.Provider == null)
            //{
            //    Options.Provider = new OAuthBearerAuthenticationProvider();
            //}

            if (Options.AccessTokenFormat == null)
            {
                IDataProtector dataProtecter = app.CreateDataProtector(
                    typeof(OAuthBearerAuthenticationMiddleware).Namespace,
                    "Access_Token", "v1");
                Options.AccessTokenFormat = new TicketDataFormat(dataProtecter);
            }

            if (Options.AccessTokenProvider == null)
            {
                Options.AccessTokenProvider = new AuthenticationTokenProvider();
            }
        }

        /// <summary>
        /// Called by the AuthenticationMiddleware base class to create a per-request handler. 
        /// </summary>
        /// <returns>A new instance of the request handler</returns>
        protected override AuthenticationHandler<OAuthTokenAuthenticationOptions> CreateHandler()
        {
            return new OAuthTokenAuthenticationHandler( _challenge);
        }
    }
}
