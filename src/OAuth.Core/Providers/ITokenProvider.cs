using Dongbo.OAuth.Core.Models;
using Dongbo.OAuth.Core.Providers;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public interface ITokenProvider
    {
        /// <summary>
        /// Creates an authorization code.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<TokenCreationResult<IAuthorizationCode>> CreateAuthorizationCode(AuthenticationTokenCreateContext context);

        /// <summary>
        /// Validates an authorization code.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ValidateAuthorizationCode(OAuthValidateTokenContext<IAuthorizationCode> context);

        /// <summary>
        /// Creates an access token.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<TokenCreationResult<IAccessToken>> CreateAccessToken(AuthenticationTokenCreateContext context);

        /// <summary>
        /// Validates the access token.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ValidateAccessToken(OAuthValidateTokenContext<IAccessToken> context);

        /// <summary>
        /// Creates a refresh token.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<TokenCreationResult<IRefreshToken>> CreateRefreshToken(AuthenticationTokenCreateContext context);

        /// <summary>
        /// Validates the refresh token.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ValidateRefreshToken(OAuthValidateTokenContext<IRefreshToken> context);
    }
}
