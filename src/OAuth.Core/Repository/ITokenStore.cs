using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{

    public interface ITokenStore
    {
        /// <summary>
        /// Deletes all access tokens, refresh tokens and authorization codes.
        /// </summary>
        /// <returns>success?true:false</returns>
        Task<bool> Purge();
    }
    public interface IAccessTokenStore : ITokenStore
    {
        Task<IAccessToken> GetAccessToken(string identifier);

        Task<IEnumerable<IAccessToken>> GetAccessTokens(DateTimeOffset expires);

        Task<IEnumerable<IAccessToken>> GetAccessTokens(string subject, DateTimeOffset expires);

        Task<IAccessToken> InsertAccessToken(IAccessToken accessToken);

        Task<int> DeleteAccessTokens(DateTimeOffset expires);

        Task<int> DeleteAccessTokens(string clientId, string redirectUri, string subject);

        Task<bool> DeleteAccessToken(string identifier);

        Task<bool> DeleteAccessToken(IAccessToken accessToken);
    }

    public interface IAuthorizationCodeStore : ITokenStore 
    {
        Task<IAuthorizationCode> GetAuthorizationCode(string identifier);

        Task<IEnumerable<IAuthorizationCode>> GetAuthorizationCodes(string redirectUri, DateTimeOffset expires);

        Task<IAuthorizationCode> InsertAuthorizationCode(IAuthorizationCode authorizationCode);

        Task<int> DeleteAuthorizationCodes(DateTimeOffset expires);

        Task<bool> DeleteAuthorizationCode(string identifier);

        Task<bool> DeleteAuthorizationCode(IAuthorizationCode authorizationCode);
    }

    public interface IRefreshTokenStore: ITokenStore 
    {
        Task<IRefreshToken> GetRefreshToken(string identifier);

        Task<IEnumerable<IRefreshToken>> GetRefreshTokens(string clientId, string redirectUri, DateTimeOffset expires);

        Task<IEnumerable<IRefreshToken>> GetRefreshTokens(string subject, DateTimeOffset expires);

        Task<IRefreshToken> InsertRefreshToken(IRefreshToken refreshToken);

        Task<int> DeleteRefreshTokens(DateTimeOffset expires);

        Task<int> DeleteRefreshTokens(string clientId, string redirectUri, string subject);

        Task<bool> DeleteRefreshToken(string identifier);

        Task<bool> DeleteRefreshToken(IRefreshToken refreshToken);
    }
}
