using Dongbo.OAuth.Core.Models;
using Dongbo.OAuth.Core.Providers;
using Dongbo.OAuth.Core.Providers.OAuthProviders;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dongbo.OAuth.Core.OAuthTokenAuthenticationMiddleware;
namespace Dongbo.OAuth.Core
{
    public static class AppBuilderExtension
    {
        public static IAppBuilder UseDongboAuthorizationServer(this IAppBuilder app, DongboAuthorizationServerOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            // Last minute default configurations

            //if (options.IssuerUri == null)
            //{
            //    throw new InvalidOperationException("IssuerUri must be set");
            //}
            if (options.TokenProvider == null)
            {
                options.TokenProvider = new TokenProvider(options);
            }
            if (options.TokenManager == null)
            {
                throw new InvalidOperationException("TokenManager must be set");
                //options.TokenManager = new TokenManager(new TokenStore(), options.TokenProvider);
            }
            if (options.ClientManager == null)
            {
                throw new InvalidOperationException("ClientManager must be set");
                //options.ClientManager = new ClientManager(new ClientStore(), new PBKDF2CryptoProvider());
            }

            if (options.UserManager == null)
            {
                throw new InvalidOperationException("UserManager must be set");
            }


            // Initialize underlying OWIN OAuth system
            var oauthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                AccessTokenExpireTimeSpan = options.AccessTokenLifetime,
                AuthorizationCodeExpireTimeSpan = options.AuthorizationCodeLifetime,
                AuthorizeEndpointPath = new PathString(options.AuthorizationCodeEndpointUrl),
                TokenEndpointPath = new PathString(options.TokenEndpointUrl),
                Provider = new AuthorizationServerProvider(options),
                AccessTokenProvider = new AccessAuthenticationTokenProvider(options),
                AuthorizationCodeProvider = new AuthorizationCodeProvider(options),
                RefreshTokenProvider = new RefreshTokenProvider(options),
                AuthenticationType = Constants.AuthenticationType,
                AuthenticationMode = AuthenticationMode.Passive,
                //AccessTokenFormat=new Formats.JwtFormat();
            };

            app.UseOAuthAuthorizationServer(oauthOptions);

            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            //{
            //    AccessTokenFormat = oauthOptions.AccessTokenFormat,
            //    AccessTokenProvider = oauthOptions.AccessTokenProvider,
            //    AuthenticationMode = oauthOptions.AuthenticationMode,
            //    AuthenticationType = oauthOptions.AuthenticationType,
            //    Description = options.Description,
            //    Provider = new ApplicationOAuthBearerProvider(),
            //    SystemClock = options.SystemClock
            //});

            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            //{
            //    AccessTokenFormat = options.AccessTokenFormat,
            //    AccessTokenProvider = options.AccessTokenProvider,
            //    AuthenticationMode = AuthenticationMode.Passive,
            //    AuthenticationType = DefaultAuthenticationTypes.ExternalBearer,
            //    Description = options.Description,
            //    Provider = new ExternalOAuthBearerProvider(),
            //    SystemClock = options.SystemClock
            //});
            // Authentication middleware
            //app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            //{
            //    AuthenticationMode = AuthenticationMode.Active,
            //    AllowedAudiences = new[] { clientId },
            //    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
            //    {
            //        new SymmetricKeyIssuerSecurityTokenProvider(issuer, clientSecret)
            //    }
            //});


            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            {
                AccessTokenFormat = oauthOptions.AccessTokenFormat,
                //AuthenticationMode = oauthOptions.AuthenticationMode,
                AccessTokenProvider = oauthOptions.AccessTokenProvider,
                Provider = new OAuthBearerAuthenticationProvider()
                {
                    OnRequestToken = RequestToken
                }

            });
            // Set up identity endpoint
            //app.Map(options.IdentityEndpointUrl, config => config.Use<UserInfoMiddleware>());
            app.UseOAuthTokenAuthentication(new OAuthTokenAuthenticationOptions()
            {
                AccessTokenFormat = oauthOptions.AccessTokenFormat,
                //AuthenticationMode = oauthOptions.AuthenticationMode,
                AccessTokenProvider = oauthOptions.AccessTokenProvider,
                TokenEndpoint=new PathString("/o/link")
            });
            return app;
        }

        private static Task RequestToken(OAuthRequestTokenContext context)
        {
            var s = context.Token;
            return Task.FromResult<object>(null);
        }
    }
}
