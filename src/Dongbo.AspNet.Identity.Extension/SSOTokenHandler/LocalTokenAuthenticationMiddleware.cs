using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    public class LocalTokenAuthenticationMiddleware : AuthenticationMiddleware<LocalTokenAuthenticationOptions>
    {
        private readonly ILogger _logger;
       
        protected SecurityHelper Helper { get; private set; }
        public LocalTokenAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, LocalTokenAuthenticationOptions options)
            : base(next, options)
        {

            //this.Options = options;
            //if (Options.Provider == null)
            //{
            //    Options.Provider = new CookieAuthenticationProvider();
            //}
            if (String.IsNullOrEmpty(Options.CookieName))
            {
                Options.CookieName = CookieAuthenticationDefaults.CookiePrefix + Options.AuthenticationType;
            }
            if (Options.TicketDataFormat == null)
            {
                //app.GetDataProtectionProvider()
                IDataProtector dataProtector = app.CreateDataProtector(
                    typeof(LocalTokenAuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v1");

                Options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
            if (Options.CookieManager == null)
            {
                Options.CookieManager = new ChunkingCookieManager();
            }
            Options.AuthenticationMode = AuthenticationMode.Passive;
            _logger = app.CreateLogger<LocalTokenAuthenticationMiddleware>();
        }
        //public override async Task Invoke(IOwinContext context)
        //{
        //    Helper = new SecurityHelper();
        //    string cookie = "";
        //    if (context.Request.Path == TokenEndpoint)
        //    {
        //        cookie = Options.CookieManager.GetRequestCookie(context, Options.CookieName);
        //    }
        //    var signin = Helper.LookupSignIn(Options.AuthenticationType);

        //    IReadableStringCollection query = context.Request.Query;
        //    string redirectUri = query.Get(Options.ReturnUrlParameter);
        //    if (!string.IsNullOrWhiteSpace(redirectUri))
        //    {
        //        //var redirectContext = new CookieApplyRedirectContext(Context, Options, redirectUri);
        //        //Options.Provider.ApplyRedirect(redirectContext);
        //        var redirect = redirectUri + new QueryString(CallbackParameter, cookie);

        //        context.Response.Redirect(redirectUri);
        //    }
           
        //}

        protected override AuthenticationHandler<LocalTokenAuthenticationOptions> CreateHandler()
        {
            return new LocalTokenAuthenticationHandler(_logger);
        }
    }
}
