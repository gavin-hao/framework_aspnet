using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.OAuthTokenAuthenticationMiddleware
{
    
    internal class OAuthTokenAuthenticationHandler : AuthenticationHandler<OAuthTokenAuthenticationOptions>
    {
        private readonly string _challenge;

        public OAuthTokenAuthenticationHandler(string challenge)
        {
            _challenge = challenge;
        }
       
      
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            try
            {
                // Find token in default location
                string requestToken = Request.Query.Get("token");
               
                // Call provider to process the token into data
                var tokenReceiveContext = new AuthenticationTokenReceiveContext(
                    Context,
                    Options.AccessTokenFormat,
                    requestToken);

                await Options.AccessTokenProvider.ReceiveAsync(tokenReceiveContext);
                if (tokenReceiveContext.Ticket == null)
                {
                    tokenReceiveContext.DeserializeTicket(tokenReceiveContext.Token);
                }

                AuthenticationTicket ticket = tokenReceiveContext.Ticket;
                if (ticket == null)
                {
                    return null;
                }

                // Validate expiration time if present
                DateTimeOffset currentUtc = Options.SystemClock.UtcNow;

                if (ticket.Properties.ExpiresUtc.HasValue &&
                    ticket.Properties.ExpiresUtc.Value < currentUtc)
                {
                    return null;
                }

                // Give application final opportunity to override results
                bool validated = false;
                if (ticket != null &&
                    ticket.Identity != null &&
                    ticket.Identity.IsAuthenticated)
                {
                    // bearer token with identity starts validated
                    validated=true;
                }

                if (!validated)
                {
                    return null;
                }

                // resulting identity values go back to caller
                return ticket;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       
        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult<object>(null);
            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (challenge != null)
            {
                Response.Headers.AppendValues("WWW-Authenticate", _challenge);
                return Task.FromResult(0);
            }

            return Task.FromResult<object>(null);
        }
    }
}
