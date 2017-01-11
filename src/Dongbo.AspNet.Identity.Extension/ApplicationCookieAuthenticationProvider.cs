using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    public class ApplicationCookieAuthenticationProvider : CookieAuthenticationProvider
    {
        public ApplicationCookieAuthenticationProvider()
        {
            OnResponseSignIn = DoResponseSignIn;
            OnResponseSignedIn = DoResponseSignedIn;
            OnValidateIdentity = DoValidateIdentity;
            OnResponseSignOut = DoResponseSignOut;
            OnException = context => { };
        }

       
        protected virtual void DoResponseSignIn(CookieResponseSignInContext context)
        {
            //AuthenticationResponseGrant signin = Helper.LookupSignIn(Options.AuthenticationType);
            var s = context.Response.StatusCode;
        }

        protected virtual void DoResponseSignedIn(CookieResponseSignedInContext context)
        {
            
            var s = context.Response.StatusCode;
        }
        protected virtual Task DoValidateIdentity(CookieValidateIdentityContext context)
        {
            return Task.FromResult<Object>(null);
        }
        protected virtual void DoResponseSignOut(CookieResponseSignOutContext context)
        {
            var tt = context;
        }
       

      
    }

    
}
