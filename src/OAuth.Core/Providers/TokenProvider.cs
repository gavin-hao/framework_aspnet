using Dongbo.OAuth.Core.Models;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dongbo.OAuth.Core.Extensions;
namespace Dongbo.OAuth.Core.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private DongboAuthorizationServerOptions Options;
        public TokenProvider(DongboAuthorizationServerOptions options)
        {
            Options = options;
        }
        public async Task<TokenCreationResult<IAuthorizationCode>> CreateAuthorizationCode(AuthenticationTokenCreateContext context)
        {
            this.Options.Logger.DebugFormat("Creating authorization code for client '{0}' and redirect uri '{1}'", context.Request.Query["client_id"], context.Request.Query["redirect_uri"]);

            var client = context.Request.Query[Constants.Parameters.ClientId];
            var scope = !string.IsNullOrEmpty(context.Request.Query[Constants.Parameters.Scope])
                        ? context.Request.Query[Constants.Parameters.Scope].Split(' ')
                        : null;
            var redirect_uri = context.Request.Query[Constants.Parameters.RedirectUri];

            string code = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n");
            var identity = new ClaimsIdentity(context.Ticket.Identity.Claims, Constants.AuthenticationType);
            var authorizationCode = new AuthorizationCode();
            authorizationCode.ClientId = client;
            authorizationCode.ExpiresIn = DateTimeOffset.UtcNow.Add(Options.AuthorizationCodeLifetime);
            authorizationCode.Scope = scope;
            authorizationCode.RedirectUri = redirect_uri;
            authorizationCode.Subject = identity.Name;
            authorizationCode.Code = code;
            authorizationCode.IssuedAt = DateTime.Now;

            authorizationCode.Ticket = context.SerializeTicket();
            return new TokenCreationResult<IAuthorizationCode>(code, authorizationCode);
        }

        public async Task ValidateAuthorizationCode(OAuthValidateTokenContext<IAuthorizationCode> context)
        {
            var result = new TokenValidationResult<IAuthorizationCode>(context.Token);
            if (context.Token == null || string.IsNullOrWhiteSpace(context.Token.Ticket))
            {
                context.Rejected();
                return;
            }
            context.Validated();
        }

        public async Task<TokenCreationResult<IAccessToken>> CreateAccessToken(AuthenticationTokenCreateContext context)
        {


            var identity = context.Ticket.Identity;
            var clientId = context.OwinContext.GetOAuthContext().ClientId;
            var redirectUri = context.OwinContext.GetOAuthContext().RedirectUri;
            var scope = context.OwinContext.GetOAuthContext().Scope;
            var expireTime = DateTimeOffset.UtcNow.Add(Options.AccessTokenLifetime);
            string token = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n");
            // Add scope claims
            if (scope != null)
            {
                identity.RemoveClaim(p => p.Type == Constants.ClaimType.Scope);
                var newClaims = scope.Select(x => new Claim(Constants.ClaimType.Scope, x));
                identity.AddClaims(newClaims);
            }

            // Add expire claim
            identity.RemoveClaim(p => p.Type == Constants.ClaimType.Expiration);
            identity.AddClaim(new Claim(Constants.ClaimType.Expiration, expireTime.ToUnixTime().ToString()));

            var accessToken = new AccessToken()
            {
                ClientId = clientId,
                RedirectUri = redirectUri,
                Subject = identity.Name,
                Scope = scope,
                Token = token,
                Ticket = context.SerializeTicket(),
                ValidTo = expireTime
            };

            return new TokenCreationResult<IAccessToken>(token, accessToken);
        }

        public async Task ValidateAccessToken(OAuthValidateTokenContext<IAccessToken> context)
        {
            var result = new TokenValidationResult<IAccessToken>(context.Token);
            if (context.Token == null || string.IsNullOrWhiteSpace(context.Token.Ticket))
            {
                context.Rejected();
                return;
            }
            context.Validated();
        }

        public async Task<TokenCreationResult<IRefreshToken>> CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            var identity = context.Ticket.Identity;
            var expireTime = DateTimeOffset.UtcNow.Add(Options.RefreshTokenLifetime);
            var clientId = context.OwinContext.GetOAuthContext().ClientId;
            var redirectUri = context.OwinContext.GetOAuthContext().RedirectUri;
            var scope = context.OwinContext.GetOAuthContext().Scope;
            string token = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n");

            // Add expire claim
            //identity.AddClaim(new Claim(Constants.ClaimType.Expiration, expireTime.ToUnixTime().ToString()));

            var refreshToken = new RefreshToken()
            {
                ClientId = clientId,
                RedirectUri = redirectUri,
                Subject = identity.Name,
                Scope = scope,
                Token = token,
                ValidTo = expireTime,
            };

            return new TokenCreationResult<IRefreshToken>(token, refreshToken);
        }

        public async Task ValidateRefreshToken(OAuthValidateTokenContext<IRefreshToken> context)
        {
            if (!string.IsNullOrWhiteSpace(context.AuthenticationTokenReceiveContext.Token) && context.Token != null)
                context.Validated();
            else
                context.Rejected();
        }
    }
}
