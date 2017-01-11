using Dongbo.OAuth.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dongbo.Data;
using Dongbo.OAuth.Core;
using Dongbo.ApiClient;
namespace Dongbo.OAuth.Store
{
    public class TokenStore : ITokenStore, IAccessTokenStore, IRefreshTokenStore, IAuthorizationCodeStore
    {
        //public ServiceClient Service { get; private set; }
        public TokenStore()
        {
            //Service = StoreConstants.TokenServiceClient;
        }

        #region accesstoken
        public async Task<IAccessToken> GetAccessToken(string identifier)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/accesstoken?identifier={0}", identifier);
            return await Service.GetAsync<AccessToken>(uri);
           
        }

        public async Task<IEnumerable<IAccessToken>> GetAccessTokens(DateTimeOffset expires)
        {

            throw new NotImplementedException();
           
        }

        public async Task<IEnumerable<IAccessToken>> GetAccessTokens(string subject, DateTimeOffset expires)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/accesstokens?subject={0}&expires={1}", subject, expires.ToString("u"));
            return await Service.GetAsync<List<AccessToken>>(uri);

        }

        public async Task<IAccessToken> InsertAccessToken(IAccessToken accessToken)
        {
            var Service = StoreConstants.TokenServiceClient;
            var newAccessToken = new AccessToken(accessToken);
            newAccessToken.IssuedAt = DateTimeOffset.UtcNow.DateTime;
            var uri = string.Format("api/oauth/token/accesstoken/create");
            var success = await Service.PostAsync<bool, AccessToken>(uri, newAccessToken);
            return newAccessToken;
        }

        public async Task<int> DeleteAccessTokens(DateTimeOffset expires)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/accesstoken/delete?expires={0}", expires.ToString("u"));
            var result = await Service.DeleteAsync<bool>(uri);
            return 1;

        }

        public async Task<int> DeleteAccessTokens(string clientId, string redirectUri, string subject)
        {
            throw new NotImplementedException();

        }

        public async Task<bool> DeleteAccessToken(string identifier)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/accesstoken/delete?identifier={0}", identifier);
            var result = await Service.DeleteAsync<bool>(uri);
            return result;

        }

        public async Task<bool> DeleteAccessToken(IAccessToken accessToken)
        {
            var Service = StoreConstants.TokenServiceClient;
            if (accessToken == null || string.IsNullOrEmpty(accessToken.Token))
                throw new ArgumentException("accessToken.token MUST be set", "accessToken");
            return await DeleteAccessToken(accessToken.Token);

        }
        #endregion

        #region refreshtoken
        public async Task<IRefreshToken> GetRefreshToken(string identifier)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/refreshtoken?identifier={0}", identifier);
            return await Service.GetAsync<RefreshToken>(uri);

        }

        public async Task<IEnumerable<IRefreshToken>> GetRefreshTokens(string clientId, string redirectUri, DateTimeOffset expires)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IRefreshToken>> GetRefreshTokens(string subject, DateTimeOffset expires)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/refreshtokens?subject={0}&expires={1}", subject, expires.ToString("u"));
            return await Service.GetAsync<List<RefreshToken>>(uri);

        }

        public async Task<IRefreshToken> InsertRefreshToken(IRefreshToken refreshToken)
        {
            var Service = StoreConstants.TokenServiceClient;
            var newToken = new RefreshToken(refreshToken);
            newToken.IssuedAt = DateTimeOffset.UtcNow.DateTime;
            var uri = string.Format("api/oauth/token/refreshtoken/create");
            var success = await Service.PostAsync<bool, RefreshToken>(uri, newToken);
            return newToken;
        }

        public async Task<int> DeleteRefreshTokens(DateTimeOffset expires)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/refreshtoken/delete?expires={0}", expires.ToString("u"));
            var result = await Service.DeleteAsync<int>(uri);
            return result;

        }

        public async Task<int> DeleteRefreshTokens(string clientId, string redirectUri, string subject)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteRefreshToken(string identifier)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/refreshtoken/delete?identifier={0}", identifier);
            var result = await Service.DeleteAsync<bool>(uri);
            return result;

        }

        public async Task<bool> DeleteRefreshToken(IRefreshToken refreshToken)
        {
            var Service = StoreConstants.TokenServiceClient;
            if (refreshToken == null || string.IsNullOrEmpty(refreshToken.Token))
                throw new ArgumentException("refreshToken.token MUST be set", "refreshToken");
            return await DeleteRefreshToken(refreshToken.Token);
        }
        #endregion

        #region authorization code
        public async Task<IAuthorizationCode> GetAuthorizationCode(string identifier)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/authorizationcode?identifier={0}", identifier);
            return await Service.GetAsync<AuthorizationCode>(uri);

        }

        public async Task<IEnumerable<IAuthorizationCode>> GetAuthorizationCodes(string redirectUri, DateTimeOffset expires)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/authorizationcodes?subject={0}&expires={1}", redirectUri, expires.ToString("u"));
            return await Service.GetAsync<List<AuthorizationCode>>(uri);

        }

        public async Task<IAuthorizationCode> InsertAuthorizationCode(IAuthorizationCode authorizationCode)
        {
            var Service = StoreConstants.TokenServiceClient;
            var newToken = new AuthorizationCode(authorizationCode);
            newToken.IssuedAt = DateTimeOffset.UtcNow.DateTime;
            var uri = string.Format("api/oauth/token/authorizationcode/create");
            var success = await Service.PostAsync<bool, AuthorizationCode>(uri, newToken);
            return newToken;


        }

        public async Task<int> DeleteAuthorizationCodes(DateTimeOffset expires)
        {
           
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/authorizationcode/delete?expires={0}", expires.ToString("u"));
            var result = await Service.DeleteAsync<int>(uri);
            return result;

        }

        public async Task<bool> DeleteAuthorizationCode(string identifier)
        {
            var Service = StoreConstants.TokenServiceClient;
            var uri = string.Format("api/oauth/token/authorizationcode/delete?identifier={0}", identifier);
            var result = await Service.DeleteAsync<bool>(uri);
            return result;


        }

        public async Task<bool> DeleteAuthorizationCode(IAuthorizationCode authorizationCode)
        {
            if (authorizationCode == null || string.IsNullOrEmpty(authorizationCode.Code))
                throw new ArgumentException("authorizationCode.Code MUST be set", "authorizationCode");
            return await DeleteAuthorizationCode(authorizationCode.Code);
        }
        #endregion
        public Task<bool> Purge()
        {
            throw new NotImplementedException();
        }


    }
}
