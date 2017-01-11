using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Logging;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using System.Net.Http;
using Microsoft.Owin.Infrastructure;
namespace Dongbo.Owin.Security.Passport
{
    public class DongboPassportAuthenticationMiddleware : AuthenticationMiddleware<DongboPassportAuthenticationOptions>
    {
        public DongboPassportAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, DongboPassportAuthenticationOptions options)
            : base(next, options)
        {

        }

        protected override AuthenticationHandler<DongboPassportAuthenticationOptions> CreateHandler()
        {
            return new DongboPassportAuthenticationHandler();
        }
    }
}
