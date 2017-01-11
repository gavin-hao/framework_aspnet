using Dongbo.OAuth.Core.Models;
using Dongbo.OAuth.Core.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dongbo.OAuth.Core.Providers.OAuthProviders
{
    public class RefreshTokenProvider : AuthenticationTokenProvider
    {
        private DongboAuthorizationServerOptions options;
        public RefreshTokenProvider(DongboAuthorizationServerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;

            this.OnCreate += this.CreateRefreshToken;
            this.OnReceive += this.ReceiveRefreshToken;
        }

        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            this.options.Logger.Debug("Received refresh token");

            AsyncHelper.RunSync(() => PopulateAuthenticationTicket(context));

            this.options.Logger.Debug("Finished processing refresh token");
        }
        private async Task PopulateAuthenticationTicket(AuthenticationTokenReceiveContext context)
        {
            var token = await options.TokenManager.FindRefreshTokenAsync(context.Token);
            OAuthValidateTokenContext<IRefreshToken> validateContext = new OAuthValidateTokenContext<IRefreshToken>(
               context.OwinContext, options, context, token);

            await options.TokenProvider.ValidateRefreshToken(validateContext);
            if (validateContext.IsValidated)
            {
                //if (token == null)//delete this section after bug fixed
                //{
                //    token = new RefreshToken { Subject = "751523633@qq.com",Scope=new List<string>(), Token = "2bchanghailong", ClientId = "win64", RedirectUri = "app://axe/token.htm", ExpiresIn = DateTime.UtcNow.AddYears(1) };
                //}
                var deleteResult = await options.TokenManager.DeleteRefreshTokenAsync(token);
                if (!deleteResult)
                {
                    options.Logger.ErrorFormat("Unable to delete used refresh token: {0}", JsonConvert.SerializeObject(token));
                }
                /* Override the validation parameters.
                           * This is because OWIN thinks the principal.Identity.Name should 
                           * be the same as the client_id from ValidateClientAuthentication method,
                           * but we need to use the user id in dongbo oauth.
                           */
                var props = new AuthenticationProperties();
                props.Dictionary.Add("client_id", token.ClientId);
                props.RedirectUri = token.RedirectUri;
                props.ExpiresUtc = token.ValidTo;

                // Re-authenticate user to get new claims
                var user = await this.options.UserManager.AuthenticateUserAsync(token.Subject);
                // Make sure the user has the correct claims
                user.RemoveClaim(x => x.Type == Constants.ClaimType.Client);
                user.RemoveClaim(x => x.Type == Constants.ClaimType.RedirectUri);
                user.AddClaim(Constants.ClaimType.Client, token.ClientId);
                user.AddClaim(Constants.ClaimType.RedirectUri, token.RedirectUri);

                if (token.Scope != null)
                {
                    foreach (var s in token.Scope)
                    {
                        user.AddClaim(Constants.ClaimType.Scope, s);
                    }
                }
                var ticket = new AuthenticationTicket(user, props);
                context.SetTicket(ticket);
            }


        }
        private void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {


            this.options.Logger.DebugFormat(
                "Creating refresh token for user '{0}', client id '{1}' and redirect uri '{2}'",
                context.Ticket.Identity.Name,
                context.OwinContext.GetOAuthContext().ClientId,
                context.OwinContext.GetOAuthContext().RedirectUri);

            var refreshToken = AsyncHelper.RunSync(() => IssuedRefreshToken(context));
            if (refreshToken != null)
                context.SetToken(refreshToken);

            this.options.Logger.Debug("Created refresh token");
        }

        private async Task<string> IssuedRefreshToken(AuthenticationTokenCreateContext context)
        {

            // Dont create a refresh token if it is a client_credentials request
            if (context.OwinContext.GetOAuthContext().GrantType == Constants.GrantTypes.ClientCredentials)
            {
                this.options.Logger.Debug("This is a client_credentials request, skipping refresh token creation.");

                return null;
            }

            var createResult = await this.options.TokenProvider.CreateRefreshToken(context);
            var result = await options.TokenManager.CreateRefreshTokenAsync(createResult.Entity);
            if (result == null)
                return null;
            return createResult.Token;

        }
    }
}
