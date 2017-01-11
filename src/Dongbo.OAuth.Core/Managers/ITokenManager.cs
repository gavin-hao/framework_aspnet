using Dongbo.OAuth.Core.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public interface ITokenManager : ITokenManager<IAccessToken, IRefreshToken, IAuthorizationCode> { }
    public interface ITokenManager<TAccessToken, TRefreshToken, TAuthorizationCode>
        where TAccessToken : IAccessToken
        where TRefreshToken : IRefreshToken
        where TAuthorizationCode : IAuthorizationCode
    {
        Task<TAuthorizationCode> FindAuthorizationCodeAsync(string code);
        Task<TAuthorizationCode> CreateAuthorizationCodeAsync(TAuthorizationCode code);
        Task<bool> DeleteAuthorizationCode(TAuthorizationCode code);



        Task<TAccessToken> FindAccessTokenAsync(string accessToken);

        Task<bool> DeleteAccessTokenAsync(TAccessToken accessToken);

        Task<TAccessToken> CreateAccessTokenAsync(TAccessToken accessToken);


        Task<TRefreshToken> FindRefreshTokenAsync(string refreshToken);

        Task<bool> DeleteRefreshTokenAsync(TRefreshToken refreshToken);

        Task<TRefreshToken> CreateRefreshTokenAsync(TRefreshToken refreshToken);


        ///// <summary>
        ///// Authenticates the authorization code.
        ///// </summary>
        //Task<TAuthorizationCode> AuthenticateAuthorizationCodeAsync(string code);

        ///// <summary>
        ///// Authenticates the access token.
        ///// </summary>
        ///// <param name="accessToken"></param>
        ///// <returns></returns>
        //Task<AuthenticationTicket> AuthenticateAccessTokenAsync(string accessToken);

        ///// <summary>
        ///// Authenticates the refresh token.
        ///// </summary>
        ///// <param name="clientId">The client id.</param>
        ///// <param name="redirectUri">The redirect URI.</param>
        ///// <param name="refreshToken">The refresh token.</param>
        ///// <returns>The user principal.</returns>
        //Task<AuthenticationTicket> AuthenticateRefreshTokenAsync(string clientId, string redirectUri, string refreshToken);

        ///// <summary>
        ///// Generates an authorization code for the specified client.
        ///// </summary>
        //Task<TokenCreationResult<TAuthorizationCode>> CreateAuthorizationCodeAsync(TAuthorizationCode authorizationCode);

        ///// <summary>Creates an access token.</summary>
        ///// <param name="userPrincipal">The user principal.</param>
        ///// <param name="expire">The expire time.</param>
        ///// <param name="clientId">The client id.</param>
        ///// <param name="redirectUri">The redirect URI.</param>
        ///// <param name="scope">The scope.</param>
        ///// <returns>The token creation result.</returns>
        //Task<TokenCreationResult<TAccessToken>> CreateAccessTokenAsync(ClaimsIdentity userPrincipal, TimeSpan expire, string clientId, string redirectUri, IEnumerable<string> scope);

        ///// <summary>Creates a refresh token.</summary>
        ///// <param name="userPrincipal">The principal.</param>
        ///// <param name="expire">The expire time.</param>
        ///// <param name="clientId">The client id.</param>
        ///// <param name="redirectUri">The redirect URI.</param>
        ///// <param name="scope">The scope.</param>
        ///// <returns>The token creation result.</returns>
        //Task<TokenCreationResult<TRefreshToken>> CreateRefreshTokenAsync(ClaimsIdentity userPrincipal, TimeSpan expire, string clientId, string redirectUri, IEnumerable<string> scope);
    }
}
