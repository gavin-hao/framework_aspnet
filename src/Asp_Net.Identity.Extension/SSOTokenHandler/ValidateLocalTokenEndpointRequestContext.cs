using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    internal static class Parameters
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
    public class ValidateLocalTokenEndpointRequestContext : BaseContext<LocalTokenAuthenticationOptions>
    {
        public ValidateLocalTokenEndpointRequestContext(
            IOwinContext context, LocalTokenAuthenticationOptions options)
            : base(context, options)
        {
            IReadableStringCollection parameters = Request.Query;
            foreach (var parameter in parameters)
            {
                AddParameter(parameter.Key, parameters.Get(parameter.Key));
            }
        }
        private void AddParameter(string name, string value)
        {
            if (string.Equals(name, Parameters.ResponseType, StringComparison.Ordinal))
            {
                ResponseType = value;
            }
            else if (string.Equals(name, Parameters.ClientId, StringComparison.Ordinal))
            {
                ClientId = value;
            }
            else if (string.Equals(name, Parameters.RedirectUri, StringComparison.Ordinal))
            {
                RedirectUri = value;
            }
            else if (string.Equals(name, Parameters.State, StringComparison.Ordinal))
            {
                State = value;
            }

        }

        public string ResponseType { get; set; }

        public string ClientId { get; set; }

        public string RedirectUri { get; set; }

        public string State { get; set; }

        public bool IsValid { get; private set; }
        public bool Validated()
        {
            if (!string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ResponseType)
                && !string.IsNullOrWhiteSpace(RedirectUri) && !string.IsNullOrWhiteSpace(State))
            {
                IsValid = true;
            }
            else
            {
                IsValid = false;
            }
            return IsValid;
        }


    }
}
