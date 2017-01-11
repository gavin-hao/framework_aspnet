using Microsoft.Owin;
using Microsoft.Owin.Helpers;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Owin.Security.Passport
{
    internal class DongboPassportAuthenticationHandler : AuthenticationHandler<DongboPassportAuthenticationOptions>
    {
        public DongboPassportAuthenticationHandler()
        {

        }
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return Task.FromResult<AuthenticationTicket>(null);
        }

        /// <summary>
        ///  Invoked to detect and process incoming authentication requests.
        /// </summary>
        /// <returns></returns>
        public override Task<bool> InvokeAsync()
        {
            return InvokeReplyPathAsync();
        }
        /// <summary>
        ///  Returns true if the request was handled, false if the next middleware should be invoked.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InvokeReplyPathAsync()
        {
            //apply /login redirect to dongbo passport
            if (Request.Path == Options.LoginPath)
            {
                IReadableStringCollection query = Request.Query;
                string redirectUri = query.Get(Options.ReturnUrlParameter);

                if (String.IsNullOrWhiteSpace(redirectUri))
                {
                    redirectUri = Request.Scheme +
                           Uri.SchemeDelimiter +
                           Request.Host +
                           Request.PathBase +
                           "/";
                }
                else
                {
                    //var reg = new System.Text.RegularExpressions.Regex("http(s?)://.*");
                    //var abs=reg.IsMatch(redirectUri);
                    //var uri = new Uri(redirectUri);
                    if (IsHostRelative(redirectUri))
                    {
                        redirectUri = Request.Scheme +
                            Uri.SchemeDelimiter +
                            Request.Host + redirectUri;

                    }
                }
                string loginUri =
                           Options.AuthorizationEndpoint +
                           new QueryString(Options.ReturnUrlParameter, redirectUri);
                loginUri = WebUtilities.AddQueryString(loginUri, "appid", Options.AppId);
                Response.Redirect(loginUri);
                return true;
            }
            if (Request.Path == Options.LogoutPath && Request.Method.ToLower() == "post")
            {
                var redirectUri = Request.Scheme +
                           Uri.SchemeDelimiter +
                           Request.Host +
                           Request.PathBase +
                           "/";
                string logoutUri =
                          Options.SignOutEndpoint +
                          new QueryString(Options.ReturnUrlParameter, redirectUri);
                Response.Redirect(logoutUri);
                return true;
            }
            if (Request.Path == Options.SignupPath)
            {
                var redirectUri = Request.Scheme +
                           Uri.SchemeDelimiter +
                           Request.Host +
                           Request.PathBase +
                           "/";
                string signup =
                          Options.SignupEndpoint +
                          new QueryString(Options.ReturnUrlParameter, redirectUri);
                signup = WebUtilities.AddQueryString(signup, "appid", Options.AppId);
                Response.Redirect(signup);
                return true;
            }
            return false;
        }

        private static bool IsHostRelative(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            if (path.Length == 1)
            {
                return path[0] == '/';
            }
            return path[0] == '/' && path[1] != '/' && path[1] != '\\';
        }

    }
}
