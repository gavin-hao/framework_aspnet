using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Providers
{
    public class OAuthValidateTokenContext<TToken> : BaseValidatingContext<DongboAuthorizationServerOptions>
    {
        public AuthenticationTokenReceiveContext AuthenticationTokenReceiveContext { get; private set; }
        public TToken Token { get; set; }
        public OAuthValidateTokenContext(IOwinContext context, DongboAuthorizationServerOptions options,
            AuthenticationTokenReceiveContext tokenContext, TToken token)
            : base(context, options)
        {
            AuthenticationTokenReceiveContext = tokenContext;
            Token = token;
        }
    }
}
