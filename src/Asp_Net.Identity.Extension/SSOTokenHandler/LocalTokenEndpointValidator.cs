using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{

    internal class LocalTokenReturnEndpointContext : ReturnEndpointContext
    {
        public LocalTokenReturnEndpointContext(
            IOwinContext context,
            AuthenticationTicket ticket, LocalTokenAuthenticationOptions options)
            : base(context, ticket)
        {
            Options = options;
        }

        public LocalTokenAuthenticationOptions Options { get; set; }
    }
    internal class LocalTokenReturnEndpointValidator
    {
        public LocalTokenReturnEndpointValidator(LocalTokenReturnEndpointContext context)
        {

            Context = context;
        }
        LocalTokenReturnEndpointContext Context { get; set; }
        public async Task<bool> Validate()
        {
            var ok = false;
            try
            {
                ok= !Context.IsRequestCompleted && Context.RedirectUri != null && await VerifyCallerHost();
            }
            catch
            {
                ok = false;
            }
            return ok;
           
        }

        private async Task<bool> VerifyCallerHost()
        {
            if (Context.Options.ClientHostsWhitelists != null && Context.Options.ClientHostsWhitelists.Count > 0)
            {
                var toValid = new Uri(Context.RedirectUri).Host;
                var all = Context.Options.ClientHostsWhitelists.FirstOrDefault(p => p == "*");
                if (!string.IsNullOrWhiteSpace(all))
                    return true;
                var host = Context.Options.ClientHostsWhitelists.FirstOrDefault(h => String.Equals(toValid, h));
                return !string.IsNullOrWhiteSpace(host);
            }
            return true;
        }
    }
}
