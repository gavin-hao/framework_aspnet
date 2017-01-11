using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    public class LocalTokenAuthenticationOptions : AuthenticationOptions
    {
        //public LocalTokenAuthenticationOptions()
        //    : base(CookieAuthenticationDefaults.AuthenticationType)
        //{
        //    TokenEndpoint = new PathString("/get_token");
        //    CallbackTokenParameter = "token";

        //}

        public LocalTokenAuthenticationOptions(CookieAuthenticationOptions cookieOptions)
            : base(cookieOptions.AuthenticationType)
        {
            TokenEndpoint = new PathString("/get_token");
            CallbackTokenParameter = "access_token";
            ReturnUrlParameter = "redirect_uri";
            CookieManager = cookieOptions.CookieManager;
            SessionStore = cookieOptions.SessionStore;
            TicketDataFormat = cookieOptions.TicketDataFormat;
            CookieName = cookieOptions.CookieName;
            SystemClock = cookieOptions.SystemClock;
            ClientHostsWhitelists = new List<string>() { "*" };
        }
        public PathString TokenEndpoint { get; set; }
        public string ReturnUrlParameter { get; private set; }

        public string CallbackTokenParameter { get; private set; }

        public ICookieManager CookieManager { get; set; }

        public IAuthenticationSessionStore SessionStore { get; set; }

        public ISecureDataFormat<AuthenticationTicket> TicketDataFormat { get; set; }
        public ISecureDataFormat<AuthenticationProperties> TokenDataFormat { get; set; }
        public string CookieName { get; set; }

        public ISystemClock SystemClock { get; set; }

        public List<string> ClientHostsWhitelists { get; set; }
    }
}
