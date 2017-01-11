using Dongbo.OAuth.Core.Models;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dongbo.OAuth.Core.Extensions;
using Microsoft.Owin.Security;
namespace Dongbo.OAuth.Core.Providers.OAuthProviders
{
    public class AuthorizationCodeProvider : AuthenticationTokenProvider
    {
        private DongboAuthorizationServerOptions options;
        public AuthorizationCodeProvider(DongboAuthorizationServerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;

            this.OnCreate = this.CreateAuthenticationCode;
            this.OnReceive = this.ReceiveAuthenticationCode;
        }

        private void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            //context.Token
            AsyncHelper.RunSync(() => PopulateAuthenticationTicket(context));

        }
        private async Task PopulateAuthenticationTicket(AuthenticationTokenReceiveContext context)
        {


            var parameters = await context.Request.ReadFormAsync();

            this.options.Logger.DebugFormat("Validating authorization code for redirect uri '{0}'", parameters["redirect_uri"]);

            var code = await this.options.TokenManager.FindAuthorizationCodeAsync(context.Token);
            OAuthValidateTokenContext<IAuthorizationCode> validateContext = new OAuthValidateTokenContext<IAuthorizationCode>(context.OwinContext, options, context, code);

            await options.TokenProvider.ValidateAuthorizationCode(validateContext);
            if (validateContext.IsValidated)
            {
                context.DeserializeTicket(code.Ticket);
            }

        }
        private void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            
            var token = AsyncHelper.RunSync<string>(() => IssuedCode(context));
            if (token != null)
            {
                context.SetToken(token);

                this.options.Logger.Debug("Created authorization code");
            }
          
        }

        private async Task<string> IssuedCode(AuthenticationTokenCreateContext context)
        {
           

            var result = await options.TokenProvider.CreateAuthorizationCode(context);

           
            // Generate code
            var createResult =await this.options.TokenManager.CreateAuthorizationCodeAsync(result.Entity);
            if (createResult == null)
            {
                return null;
            }
            return result.Token;
        }
    }
}
