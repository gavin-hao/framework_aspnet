using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    /// <summary>
    ///handle the sso client  get_token request,if authenticated then redirect to cient redirect_uri and append current authentication cookie to the redirect parameter seems like  "redirect_uri?token"
    /// </summary>
    public class LocalTokenAuthenticationHandler : AuthenticationHandler<LocalTokenAuthenticationOptions>
    {

        private const string SessionIdClaim = "Microsoft.Owin.Security.Cookies-SessionId";
        private readonly ILogger _logger;
        private string _sessionKey;
        public LocalTokenAuthenticationHandler(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _logger = logger;
        }
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationTicket ticket = null;
            IReadableStringCollection query = Request.Query;
            var redirectUrl = query.Get(Options.ReturnUrlParameter);
            var state = query.Get("state");
            var grantType = query.Get("");


            try
            {
                string cookie = Options.CookieManager.GetRequestCookie(Context, Options.CookieName);
                if (string.IsNullOrWhiteSpace(cookie))
                {
                    return await GenerateEmptyTiket(redirectUrl);
                }

                ticket = Options.TicketDataFormat.Unprotect(cookie);
                if (ticket == null)
                {
                    _logger.WriteWarning(@"Unprotect ticket failed");
                    return await GenerateEmptyTiket(redirectUrl);
                }
                if (Options.SessionStore != null)
                {
                    Claim claim = ticket.Identity.Claims.FirstOrDefault(c => c.Type.Equals(SessionIdClaim));
                    if (claim == null)
                    {
                        _logger.WriteWarning(@"SessoinId missing");
                        return await GenerateEmptyTiket(redirectUrl);
                    }
                    _sessionKey = claim.Value;
                    ticket = await Options.SessionStore.RetrieveAsync(_sessionKey);
                    if (ticket == null)
                    {
                        _logger.WriteWarning(@"Identity missing in session store");
                        return await GenerateEmptyTiket(redirectUrl);
                    }
                }
                DateTimeOffset currentUtc = Options.SystemClock.UtcNow;
                DateTimeOffset? issuedUtc = ticket.Properties.IssuedUtc;
                DateTimeOffset? expiresUtc = ticket.Properties.ExpiresUtc;

                if (expiresUtc != null && expiresUtc.Value < currentUtc)
                {
                    return await GenerateEmptyTiket(redirectUrl);
                }
                ticket.Properties.RedirectUri = redirectUrl;
                return ticket;
            }
            catch (Exception exception)
            {

                _logger.WriteError("Authenticate error", exception);
                ticket = new AuthenticationTicket(null, new AuthenticationProperties());
                ticket.Properties.RedirectUri = redirectUrl;
                return ticket;
            }
        }

        private async Task<AuthenticationTicket> GenerateEmptyTiket(string redirectUrl)
        {
            var ticket = new AuthenticationTicket(null, new AuthenticationProperties());
            ticket.Properties.RedirectUri = redirectUrl;
            return ticket;
        }
        public override async Task<bool> InvokeAsync()
        {
            return await InvokeReplyPathAsync();
        }
        private async Task<bool> InvokeReplyPathAsync()
        {
            // retrive auth cookie return to client
            if (Options.TokenEndpoint.HasValue && Options.TokenEndpoint == Request.Path)
            {
                var validateContext = new ValidateLocalTokenEndpointRequestContext(Context, Options);
                if (!validateContext.Validated())
                {
                    _logger.WriteWarning("LocalTokenAuthenticationHandler -->invalid_request");
                    Response.StatusCode = 400;
                    Response.ContentType = "application/json";
                    Response.Headers["WWW-Authenticate"] = string.Empty;
                    await Response.WriteAsync("invalid_request");
                    return true;
                }

                var ticket = await AuthenticateAsync();
                var token = "";
                if (ticket.Identity != null)
                {
                    token = Options.TicketDataFormat.Protect(ticket);
                }
                var parameters=new Dictionary<string,string>();
                parameters.Add(validateContext.ResponseType,token);
                parameters.Add(Parameters.State,validateContext.State);
                string redirectUri = WebUtilities.AddQueryString(validateContext.RedirectUri,parameters);

                Response.Redirect(redirectUri);
                return true;
            }
            return false;
        }

        protected override Task ApplyResponseCoreAsync()
        {
            return base.ApplyResponseCoreAsync();
        }

        protected override Task ApplyResponseGrantAsync()
        {
            return base.ApplyResponseGrantAsync();
        }
    }
}
