using Dongbo.OAuth.Core.Models;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dongbo.OAuth.Core.Extensions;
using System.Security.Claims;
using Microsoft.Owin.Security;
namespace Dongbo.OAuth.Core.Providers
{
    public class AccessAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        private DongboAuthorizationServerOptions options;

        public AccessAuthenticationTokenProvider(DongboAuthorizationServerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            this.options = options;

            this.OnCreate = this.CreateAccessToken;
            this.OnReceive = this.ReceiveAccessToken;
        }

        private void ReceiveAccessToken(AuthenticationTokenReceiveContext context)
        {
            this.options.Logger.Debug("Received access token");
            AsyncHelper.RunSync(() => CreateAuthenticationTicket(context));

            //if (ticket != null)
            //{
            //    context.SetTicket(ticket);
            //}

            this.options.Logger.Debug("Finished processing access token");

        }
        private async Task CreateAuthenticationTicket(AuthenticationTokenReceiveContext context)
        {
            var token = await this.options.TokenManager.FindAccessTokenAsync(context.Token);
            OAuthValidateTokenContext<IAccessToken> validateContext = new OAuthValidateTokenContext<IAccessToken>(
                context.OwinContext, options, context, token);

            await options.TokenProvider.ValidateAccessToken(validateContext);
            if (validateContext.IsValidated)
            {
                context.DeserializeTicket(token.Ticket);
            }

            //var identity = await options.TokenManager.AuthenticateAccessTokenAsync(context.Token);
            //if (identity.IsAuthenticated)
            //{

            //    var props = new AuthenticationProperties
            //    {
            //        ExpiresUtc = DateTimeOffset.UtcNow.Add(this.options.AccessTokenLifetime)
            //    };

            //    if (identity.HasClaim(x => x.Type == Constants.ClaimType.Client))
            //    {
            //        props.Dictionary.Add("client_id", identity.Claims.First(x => x.Type == Constants.ClaimType.Client).Value);
            //    }

            //    return new AuthenticationTicket(identity, props);
            //}
            //else
            //{
            //    return null;
            //}
        }
        private void CreateAccessToken(AuthenticationTokenCreateContext context)
        {
            string accessToken;


            if (context.OwinContext.GetOAuthContext().GrantType == Constants.GrantTypes.ClientCredentials)
            {
                this.options.Logger.DebugFormat(
                   "Creating access token for client '{0}' and scope '{1}'",
                   context.Ticket.Identity.Name,
                   string.Join(", ", context.OwinContext.GetOAuthContext().Scope));


                accessToken = AsyncHelper.RunSync<string>(() => IssueAccessToken(context));

            }
            else
            {
                this.options.Logger.DebugFormat(
                   "Creating access token for user '{0}', client id '{1}' and redirect uri '{2}'",
                   context.Ticket.Identity.Name,
                   context.OwinContext.GetOAuthContext().ClientId,
                   context.OwinContext.GetOAuthContext().RedirectUri);

                accessToken = AsyncHelper.RunSync<string>(() => IssueAccessToken(context));
            }
            if (!string.IsNullOrWhiteSpace(accessToken))
                context.SetToken(accessToken);

            this.options.Logger.Debug("Created access token");
        }

        private async Task<string> IssueAccessToken(AuthenticationTokenCreateContext context)
        {

            //var createResult = await this.options.TokenManager.CreateAccessTokenAsync(
            //                        context.Ticket.Identity,
            //                        this.options.AccessTokenLifetime,
            //                        context.OwinContext.GetOAuthContext().ClientId,
            //                        context.OwinContext.GetOAuthContext().RedirectUri,
            //                        context.OwinContext.GetOAuthContext().Scope);
            var createResult = await options.TokenProvider.CreateAccessToken(context);

            // Store id token in context if scope contains openid
            if (context.OwinContext.GetOAuthContext().Scope.Contains("openid"))
            {
                context.OwinContext.GetOAuthContext().IdToken = createResult.Entity.Ticket;
            }

            // Generate code
            var accessToken = await this.options.TokenManager.CreateAccessTokenAsync(createResult.Entity);
            if (accessToken == null)
            {
                return null;
            }
            return createResult.Token;
        }
    }
}
