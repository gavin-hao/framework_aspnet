using Dongbo.OAuth.Core.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dongbo.OAuth.Core.Extensions;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Text.RegularExpressions;
using Microsoft.Owin.Security.Infrastructure;

namespace Dongbo.OAuth.Core.Providers.OAuthProviders
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly DongboAuthorizationServerOptions options;
        public AuthorizationServerProvider(DongboAuthorizationServerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;


        }
        /// <summary>
        /// Called for each request to the Token endpoint to determine if the request is valid and should continue. 
        /// The default behavior when using the OAuthAuthorizationServerProvider is to assume well-formed requests, with 
        /// validated client credentials, should continue processing. An application may add any additional constraints.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            this.options.Logger.Debug("Token request is valid");

            // Store grant type in context
            context.OwinContext.GetOAuthContext().GrantType = context.TokenRequest.GrantType;

            context.Validated();
        }
        /// <summary>
        /// Called for each request to the Authorize endpoint to determine if the request is valid and should continue. 
        /// The default behavior when using the OAuthAuthorizationServerProvider is to assume well-formed requests, with 
        /// validated client redirect URI, should continue processing. An application may add any additional constraints.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            this.options.Logger.Debug("Authorize request is valid");

            context.Validated();

        }
        /// <summary>
        /// Called to validate that the context.ClientId is a registered "client_id", and that the context.RedirectUri a "redirect_uri" 
        /// registered for that client. This only occurs when processing the Authorize endpoint. The application MUST implement this
        /// call, and it MUST validate both of those factors before calling context.Validated. If the context.Validated method is called
        /// with a given redirectUri parameter, then IsValidated will only become true if the incoming redirect URI matches the given redirect URI. 
        /// If context.Validated is not called the request will not proceed further. 
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            this.options.Logger.Debug("Validating client id and redirect uri");

            // Only proceed if client id and redirect uri is provided
            if (string.IsNullOrEmpty(context.ClientId) || string.IsNullOrEmpty(context.RedirectUri))
            {
                this.options.Logger.WarnFormat("Client id ({0}) or client secret ({1}) is invalid", context.ClientId, context.RedirectUri);

                return;
            }

            this.options.Logger.DebugFormat("Authenticating client '{0}' and redirect uri '{1}'", context.ClientId, context.RedirectUri);

            var client = await this.options.ClientManager.AuthenticateClientAsync(context.ClientId, context.RedirectUri);

            if (!client.IsAuthenticated)
            {
                context.Rejected();

                this.options.Logger.WarnFormat("Client '{0}' and redirect uri '{1}' was not authenticated", context.ClientId, context.RedirectUri);

                return;
            }

            this.options.Logger.DebugFormat("Client '{0}' and redirect uri '{1}' was successfully authenticated", context.ClientId, context.RedirectUri);

            context.OwinContext.GetOAuthContext().ClientId = context.ClientId;
            context.OwinContext.GetOAuthContext().RedirectUri = context.RedirectUri;

            context.Validated(context.RedirectUri);
        }

        /// <summary>
        /// Called to validate that the origin of the request is a registered "client_id", and that the correct credentials for that client are
        /// present on the request. If the web application accepts Basic authentication credentials,
        /// context.TryGetBasicCredentials(out clientId, out clientSecret) may be called to acquire those values if present in the request header. If the web
        /// application accepts "client_id" and "client_secret" as form encoded POST parameters,
        /// context.TryGetFormCredentials(out clientId, out clientSecret) may be called to acquire those values if present in the request body.
        /// If context.Validated is not called the request will not proceed further.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            this.options.Logger.Debug("Validating client id and secret");

            string clientId;
            string clientSecret;

            // Validate that redirect uri is specified
            // 'redirect_uri' must be specified for all calls that are not 'client_credentials' grants.
            if (context.Parameters[Constants.Parameters.RedirectUri] == null && context.Parameters[Constants.Parameters.GrantType] != Constants.GrantTypes.ClientCredentials)
            {
                context.SetError("invalid_request");

                this.options.Logger.Error("Redirect URI was not specified, the token request is not valid");

                return;
            }

            if (context.TryGetBasicCredentials(out clientId, out clientSecret)
                || context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                // Only proceed if client id and client secret is provided
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    context.SetError("invalid_client");

                    this.options.Logger.WarnFormat("Client id ({0}) or client secret ({1}) is invalid", clientId, clientSecret);

                    return;
                }

                this.options.Logger.DebugFormat("Authenticating client '{0}'", clientId);

                var client = await this.options.ClientManager.AuthenticateClientCredentialsAsync(clientId, clientSecret);

                if (!client.IsAuthenticated)
                {
                    context.SetError("invalid_grant");

                    this.options.Logger.WarnFormat("Client '{0}' was not authenticated because the supplied secret did not match", clientId);

                    return;
                }
            }
            else
            {
                context.SetError("invalid_client");

                this.options.Logger.WarnFormat("Client '{0}' was not authenticated because the provider could not retrieve the client id and client secret from the Authorization header or Form parameters", clientId);

                return;
            }

            context.OwinContext.GetOAuthContext().ClientId = context.ClientId;
            context.OwinContext.GetOAuthContext().RedirectUri = context.Parameters[Constants.Parameters.RedirectUri];
            context.OwinContext.GetOAuthContext().Scope = context.Parameters[Constants.Parameters.Scope] != null && context.Parameters[Constants.Parameters.Scope].Length > 0 ?
                context.Parameters[Constants.Parameters.Scope].Split(' ') : null;

            this.options.Logger.DebugFormat("Client '{0}' was successfully authenticated", clientId);

            context.Validated(clientId);
        }

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of any other value. If the application supports custom grant types
        ///             it is entirely responsible for determining if the request should result in an access_token. If context.Validated is called with ticket
        ///             information the response body is produced in the same way as the other standard grant types. If additional response parameters must be
        ///             included they may be added in the final TokenEndpoint call.
        ///             See also http://tools.ietf.org/html/rfc6749#section-4.5
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            if (this.options.Events.UnknownGrantTypeReceived != null)
            {
                this.options.Logger.Debug("Authenticating token request using custom grant type");

                await this.options.Events.UnknownGrantTypeReceived(new UnknownGrantTypeReceivedEventArgs(context));
            }

            await base.GrantCustomExtension(context);
        }


        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "client_credentials". This occurs when a registered client
        ///             application wishes to acquire an "access_token" to interact with protected resources on it's own behalf, rather than on behalf of an authenticated user. 
        ///             If the web application supports the client credentials it may assume the context.ClientId has been validated by the ValidateClientAuthentication call.
        ///             To issue an access token the context.Validated must be called with a new ticket containing the claims about the client application which should be associated
        ///             with the access token. The application should take appropriate measures to ensure that the endpoint isn’t abused by malicious callers.
        ///             The default behavior is to reject this grant type.
        ///             See also http://tools.ietf.org/html/rfc6749#section-4.4.2
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            this.options.Logger.DebugFormat("Authenticating client credentials flow for application '{0}'", context.ClientId);

            if (context.Scope == null || !context.Scope.Any())
            {
                this.options.Logger.Warn("No scope/redirect uri was specified in the request. Request is invalid.");

                context.Rejected();

                return;
            }

            // Store scope in context
            context.OwinContext.GetOAuthContext().Scope = context.Scope;

            // Authenticate client
            var client = await this.options.ClientManager.AuthenticateClientAsync(context.ClientId, context.Scope);

            // Add grant type claim

            //var claims = client.FindAll(Constants.ClaimType.GrantType);
            //if (claims != null && claims.Count() > 0)
            //{
            //    foreach (var c in claims)
            //    {
            //        client.RemoveClaim(c);
            //    }
            //}
            client.RemoveClaim(x => x.Type == Constants.ClaimType.GrantType);
            client.AddClaim(new Claim(Constants.ClaimType.GrantType, Constants.GrantTypes.ClientCredentials));

            if (client.IsAuthenticated)
            {
                if (client.HasClaim(x => x.Type == Constants.ClaimType.RedirectUri))
                {
                    context.OwinContext.GetOAuthContext().RedirectUri = client.Claims.First(x => x.Type == Constants.ClaimType.RedirectUri).Value;
                }
                else
                {
                    this.options.Logger.Warn("Client '{context.ClientId}' does not have a valid redirect uri, validation will not work.");
                }

                var ticket = new AuthenticationTicket(client, new AuthenticationProperties());

                context.Validated(ticket);

                this.options.Logger.DebugFormat("Client '{0}' was successfully authenticated", context.ClientId);

                return;
            }

            context.Rejected();

            this.options.Logger.Warn("Client could not be authenticated");
        }

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "password". This occurs when the user has provided name and password
        /// credentials directly into the client application's user interface, and the client application is using those to acquire an "access_token" and
        /// optional "refresh_token". If the web application supports the
        /// resource owner credentials grant type it must validate the context.Username and context.Password as appropriate. To issue an
        /// access token the context.Validated must be called with a new ticket containing the claims about the resource owner which should be associated
        /// with the access token. The application should take appropriate measures to ensure that the endpoint isn’t abused by malicious callers.
        /// The default behavior is to reject this grant type.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            this.options.Logger.DebugFormat("Authenticating resource owner flow for user '{0}'", Regex.Escape(context.UserName));

            var user = await this.options.UserManager.AuthenticateUserWithPasswordAsync(context.UserName, context.Password);

            if (!user.IsAuthenticated)
            {
                context.Rejected();

                this.options.Logger.WarnFormat("User '{0}' was not authenticated", Regex.Escape(context.UserName));

                return;
            }

            // Add oauth claims
            user.AddClaim(new Claim(Constants.ClaimType.Client, context.ClientId));
            user.RemoveClaim(x => x.Type == Constants.ClaimType.GrantType);
            user.AddClaim(new Claim(Constants.ClaimType.GrantType, Constants.GrantTypes.Password));


            // Activate event if subscribed to
            if (this.options.Events.IdentityCreated != null)
            {
                var args = new IdentityCreatedEventArgs(user, context);

                await this.options.Events.IdentityCreated(args);
                if (args.Identity.AuthenticationType != context.Options.AuthenticationType)
                {
                    user = new ClaimsIdentity(args.Identity.Claims, context.Options.AuthenticationType);
                }

            }



            // Validate ticket
            var ticket = new AuthenticationTicket(user, new AuthenticationProperties());

            context.Validated(ticket);

            this.options.Logger.DebugFormat("User '{0}' was successfully authenticated", Regex.Escape(context.UserName));
        }

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "authorization_code". This occurs after the Authorize
        /// endpoint as redirected the user-agent back to the client with a "code" parameter, and the client is exchanging that for an "access_token".
        /// The claims and properties
        /// associated with the authorization code are present in the context.Ticket. The application must call context.Validated to instruct the Authorization
        /// Server middleware to issue an access token based on those claims and properties. The call to context.Validated may be given a different
        /// AuthenticationTicket or ClaimsIdentity in order to control which information flows from authorization code to access token.
        /// The default behavior when using the OAuthAuthorizationServerProvider is to flow information from the authorization code to
        /// the access token unmodified.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.1.3
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
        {
            this.options.Logger.Debug("Authenticating authorization code flow");

            var user = context.Ticket.Identity;

            // Add grant type claim
            user.RemoveClaim(x => x.Type == Constants.ClaimType.GrantType);
            user.AddClaim(new Claim(Constants.ClaimType.GrantType, Constants.GrantTypes.AuthorizationCode));

            context.Validated(user);
        }


        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "refresh_token". This occurs if your application has issued a "refresh_token" 
        ///             along with the "access_token", and the client is attempting to use the "refresh_token" to acquire a new "access_token", and possibly a new "refresh_token".
        ///             To issue a refresh token the an Options.RefreshTokenProvider must be assigned to create the value which is returned. The claims and properties 
        ///             associated with the refresh token are present in the context.Ticket. The application must call context.Validated to instruct the 
        ///             Authorization Server middleware to issue an access token based on those claims and properties. The call to context.Validated may 
        ///             be given a different AuthenticationTicket or ClaimsIdentity in order to control which information flows from the refresh token to 
        ///             the access token. The default behavior when using the OAuthAuthorizationServerProvider is to flow information from the refresh token to 
        ///             the access token unmodified.
        ///             See also http://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            this.options.Logger.Debug("Authenticating refresh token flow");

            var user = context.Ticket.Identity;

            // Add grant type claim
            user.RemoveClaim(x => x.Type == Constants.ClaimType.GrantType);
            user.AddClaim(Constants.ClaimType.GrantType, Constants.GrantTypes.RefreshToken);

            // Set scopes from refresh token
            if (user.HasClaim(x => x.Type == Constants.ClaimType.Scope))
            {
                context.OwinContext.GetOAuthContext().Scope = user.Claims.Where(x => x.Type == Constants.ClaimType.Scope).Select(x => x.Value);
            }

            // Activate event if subscribed to
            if (this.options.Events.IdentityCreated != null)
            {
                var args = new IdentityCreatedEventArgs(user, context);

                await this.options.Events.IdentityCreated(args);

                if (args.Identity.AuthenticationType != context.Options.AuthenticationType)
                {
                    user = new ClaimsIdentity(args.Identity.Claims, context.Options.AuthenticationType);
                }
            }

            context.Validated(user);
        }
        /// <summary>
        /// Called at the final stage of a successful Token endpoint request. An application may implement this call in order to do any final 
        /// modification of the claims being used to issue access or refresh tokens. This call may also be used in order to add additional 
        /// response parameters to the Token endpoint's json response body.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            return base.TokenEndpoint(context);
        }

        /// <summary>
        /// Called at the final stage of an incoming Authorize endpoint request before the execution continues on to the web application component 
        /// responsible for producing the html response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the Authorize page. If running on IIS any ASP.NET technology running on the server may produce the response for the 
        /// Authorize page. If the web application wishes to produce the response directly in the AuthorizeEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing. If the web application wishes
        /// to grant the authorization directly in the AuthorizeEndpoint call it cay call context.OwinContext.Authentication.SignIn with the
        /// appropriate ClaimsIdentity and should call context.RequestCompleted to stop other handlers from executing.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            //string uri = context.Request.Uri.ToString();

            //string rawJwt = await TryGetRawJwtTokenAsync(context);
            //if (string.IsNullOrWhiteSpace(rawJwt))
            //{
            //    context.OwinContext.Authentication.Challenge(new AuthenticationProperties { RedirectUri = uri });
            //    return;
            //}

            //var tokenValidator = new TokenValidator();
            //ClaimsPrincipal principal = tokenValidator.Validate(rawJwt, _options.JwtOptions);

            //if (!principal.Identity.IsAuthenticated)
            //{
            //    Error(context, OAuthImplicitFlowError.AccessDenied, "unauthorized user, unauthenticated");
            //    return;
            //}
            //ClaimsIdentity claimsIdentity = await _options.TransformPrincipal(principal);

            //if (!claimsIdentity.Claims.Any())
            //{
            //    Error(context, OAuthImplicitFlowError.AccessDenied, "unauthorized user");
            //    return;
            //}

            //ConsentAnswer consentAnswer = await TryGetConsentAnswerAsync(context.Request);

            //if (consentAnswer == ConsentAnswer.Rejected)
            //{
            //    Error(context, OAuthImplicitFlowError.AccessDenied, "resource owner denied request");
            //    return;
            //}

            //if (consentAnswer == ConsentAnswer.Missing)
            //{
            //    Error(context, OAuthImplicitFlowError.ServerError,
            //        "missing consent answer");
            //    return;
            //}


            //if (!(consentAnswer == ConsentAnswer.Accepted || consentAnswer == ConsentAnswer.Implicit))
            //{
            //    Error(context, OAuthImplicitFlowError.ServerError,
            //        string.Format("invalid consent answer '{0}'", consentAnswer.Display));
            //    return;
            //}
            await base.AuthorizeEndpoint(context);
        }

        /// <summary>Called before the TokenEndpoint redirects its response to the caller.</summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution.</returns>
        public override async Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            if (context.TokenIssued)
            {
                // Add id_token to output if it was set
                var idToken = context.OwinContext.GetOAuthContext().IdToken;
                if (!string.IsNullOrEmpty(idToken))
                {
                    context.AdditionalResponseParameters.Add("id_token", idToken);
                }

                if (this.options.Events.TokenIssued != null)
                {
                    await this.options.Events.TokenIssued(new TokenIssuedEventArgs(context));
                }
            }

            await base.TokenEndpointResponse(context);
        }

        /// <summary>
        /// Called before the AuthorizationEndpoint redirects its response to the caller. The response could be the
        /// token, when using implicit flow or the AuthorizationEndpoint when using authorization code flow.
        /// An application may implement this call in order to do any final modification of the claims being used
        /// to issue access or refresh tokens. This call may also be used in order to add additional
        /// response parameters to the authorization endpoint's response.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
        {
            if (!context.IsRequestCompleted && context.AuthorizeEndpointRequest.IsImplicitGrantType)
            {


                var refreshTokenCreateContext = new AuthenticationTokenCreateContext(
               context.OwinContext,
                context.Options.RefreshTokenFormat,
               new AuthenticationTicket(context.Identity, context.Properties));
                await context.Options.RefreshTokenProvider.CreateAsync(refreshTokenCreateContext);
                string refreshToken = refreshTokenCreateContext.Token;
                context.AdditionalResponseParameters.Add("refresh_token", refreshToken);
            }
            await base.AuthorizationEndpointResponse(context);
        }
    }
}
