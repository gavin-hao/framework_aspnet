using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public static class Constants
    {
        public static class Parameters
        {
            public const string ResponseType = "response_type";
            public const string GrantType = "grant_type";
            public const string ClientId = "client_id";
            public const string ClientSecret = "client_secret";
            public const string RedirectUri = "redirect_uri";
            public const string Scope = "scope";
            public const string State = "state";
            public const string Code = "code";
            public const string RefreshToken = "refresh_token";
            public const string Username = "username";
            public const string Password = "password";
            public const string Error = "error";
            public const string ErrorDescription = "error_description";
            public const string ErrorUri = "error_uri";
            public const string ExpiresIn = "expires_in";
            public const string AccessToken = "access_token";
            public const string TokenType = "token_type";

            public const string ResponseMode = "response_mode";
        }
        public static class AuthenticationMethod
        {
            /// <summary>Authenticated using client id and client secret.</summary>
            public const string ClientCredentials = "client_credentials";

            /// <summary>Authenticated using client id only.</summary>
            public const string ClientId = "client_id";

            /// <summary>Authenticated using username and password.</summary>
            public const string UserCredentials = "user_credentials";

            /// <summary>Authenticated using user id only.</summary>
            public const string UserId = "user_id";
        }
        /// <summary>
        /// The OAuth authentication method.
        /// </summary>
        public const string AuthenticationType = "OAuth";

        public static class GrantTypes
        {
            public const string Password = "password";
            public const string AuthorizationCode = "authorization_code";
            public const string ClientCredentials = "client_credentials";
            public const string RefreshToken = "refresh_token";
            public const string Implicit = "implicit";

            // assertion grants
            public const string Saml2Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";
            public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        }

        public static class ErrorCode
        {
            /// <summary>The token was invalid.</summary>
            public const string InvalidToken = "invalid_token";
        }
        public class ClaimType
        {
            /// <summary>The scope claim type.</summary>
            public const string Scope = "urn:oauth:scope";

            /// <summary>The client claim type.</summary>
            public const string Client = "urn:oauth:client";

            /// <summary>The grant type claim type.</summary>
            public const string GrantType = "urn:oauth:granttype";

            /// <summary>The redirect uri claim type.</summary>
            public const string RedirectUri = "urn:oauth:redirecturi";

            /// <summary>The id claim type.</summary>
            public const string Id = "urn:oauth:id";

            /// <summary>The issuer claim type.</summary>
            public const string Issuer = "urn:oauth:issuer";

            /// <summary>The valid from claim type.</summary>
            public const string ValidFrom = "urn:oauth:validfrom";

            /// <summary>The name claim type.</summary>
            public const string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            /// <summary>The expiration claim type.</summary>
            public const string Expiration = "http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration";

            /// <summary>The authentication instant claim type.</summary>
            public const string AuthenticationInstant = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant";
        }

        public class JwtClaimType
        {
            /// <summary>The subject claim type.</summary>
            public const string Subject = "sub";

            /// <summary>The issuer claim type.</summary>
            public const string Issuer = "iss";

            /// <summary>The audience claim type.</summary>
            public const string Audience = "aud";

            /// <summary>The expiration time claim type.</summary>
            public const string ExpirationTime = "exp";

            /// <summary>The valid from claim type.</summary>
            public const string NotBefore = "nbf";

            /// <summary>The issued at claim type.</summary>
            public const string IssuedAt = "iat";

            /// <summary>The JWT identifier claim type.</summary>
            public const string Id = "jti";

            /// <summary>The access token hash claim type.</summary>
            public const string AccessTokenHash = "at_hash";

            /// <summary>The authorization code hash claim type.</summary>
            public const string AuthorizationCodeHash = "c_hash";
        }
    }
}
