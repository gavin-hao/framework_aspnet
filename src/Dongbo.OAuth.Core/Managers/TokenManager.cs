using Dongbo.OAuth.Core.Models;
using Dongbo.OAuth.Core.Providers;
using Dongbo.OAuth.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security;

namespace Dongbo.OAuth.Core
{

    public class TokenManager : ITokenManager
    {

        private readonly Dongbo.Logging.LogWrapper logger = new Logging.LogWrapper();

        //public TokenManager(ITokenStore store){ }
        public TokenManager(ITokenStore store)
        {
            if (store == null)
                throw new ArgumentNullException("store");
            //if (tokenProvider == null)
            //    throw new ArgumentNullException("tokenProvider");
            //if (crypto == null)
            //    throw new ArgumentNullException("crypto");
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            //this.TokenProvider = tokenProvider;
            //this.CryptoProvider = crypto;
            //this.PrincipalProvider = new PrincipalProvider(crypto);
            this.Store = store;

            if (store is IAccessTokenStore)
            {
                AccessTokenStore = store as IAccessTokenStore;
            }
            if (store is IAuthorizationCodeStore)
            {
                AuthorizationCodeStore = store as IAuthorizationCodeStore;
            }
            if (store is IRefreshTokenStore)
            {
                RefreshTokenStore = store as IRefreshTokenStore;
            }

        }

        //public ITokenProvider TokenProvider { get; set; }
        public ITokenStore Store { get; set; }
        public IAccessTokenStore AccessTokenStore { get; set; }
        public IRefreshTokenStore RefreshTokenStore { get; set; }

        public IAuthorizationCodeStore AuthorizationCodeStore { get; set; }
        //public ICryptoProvider CryptoProvider { get; set; }
        //public PrincipalProvider PrincipalProvider { get; private set; }
        protected IAccessTokenStore GetAccessTokenStore()
        {
            if (AccessTokenStore == null)
                throw new NotSupportedException("IAccessTokenStore");
            return this.AccessTokenStore;
        }

        protected IRefreshTokenStore GetRefreshTokenStore()
        {
            if (AccessTokenStore == null)
                throw new NotSupportedException("IRefreshTokenStore");
            return this.RefreshTokenStore;
        }

        protected IAuthorizationCodeStore GetAuthorizationCodeStore()
        {
            if (AccessTokenStore == null)
                throw new NotSupportedException("IAuthorizationCodeStore<TAuthorizationCode>");
            return this.AuthorizationCodeStore;
        }
        protected ITokenStore GetTokenStore()
        {
            if (AccessTokenStore == null)
                throw new NotSupportedException("ITokenStore");
            return this.AuthorizationCodeStore;
        }
        //public async Task<IAuthorizationCode> AuthenticateAuthorizationCodeAsync(string code)
        //{
          

        //    //logger.DebugFormat("Authenticating authorization code '{0}' for redirect uri '{1}'", authorizationCode, redirectUri);
        //    var store = GetAuthorizationCodeStore();
        //    //var relevantCodes = store.GetAuthorizationCodes(redirectUri, DateTimeOffset.UtcNow).Result;
        //    var relevantCodes = await store.GetAuthorizationCode(code);
        //    var context=new OAuthValidateTokenContext<IAuthorizationCode>()

        //    var validationResult = await this.TokenProvider.ValidateAuthorizationCode(relevantCodes);
        //    if (validationResult.IsValidated)
        //    {
        //        this.logger.Debug("Authorization code is valid");

        //        // Delete used authorization code
        //        var deleteResult = store.DeleteAuthorizationCode(validationResult.Entity).Result;

        //        if (!deleteResult)
        //        {
        //            this.logger.Error("Unable to delete used authorization code: {JsonConvert.SerializeObject(validationResult)}");
        //        }
        //        return relevantCodes;
        //    }
        //    else
        //    {
        //        this.logger.Warn("Authorization code is not valid");
        //        return null;
        //    }
        //}

        //public async Task<ClaimsIdentity> AuthenticateAccessTokenAsync(string accessToken)
        //{

        //    this.logger.Debug("Authenticating access token");
        //    var store = GetAccessTokenStore();
        //    var relevantTokens = await store.GetAccessTokens(DateTimeOffset.UtcNow);

        //    var validationResult = await this.TokenProvider.ValidateAccessToken(relevantTokens, accessToken);

        //    if (validationResult.IsValidated)
        //    {
        //        this.logger.DebugFormat(
        //            "Access token is valid. It belongs to the user '{0}', client '{1}' and redirect uri '{2}'",
        //            validationResult.Entity.Subject,
        //            validationResult.Entity.ClientId,
        //            validationResult.Entity.RedirectUri);
        //        var identity = new ClaimsIdentity(validationResult.Principal.Claims, Constants.AuthenticationType);
        //        return identity;
        //    }

        //    this.logger.WarnFormat("Access token '{0}' is not valid", accessToken);

        //    return new ClaimsIdentity();
        //}

        //public async Task<ClaimsIdentity> AuthenticateRefreshTokenAsync(string clientId, string redirectUri, string refreshToken)
        //{
        //    this.logger.DebugFormat("Authenticating refresh token for client '{0}' and redirect uri '{1}'", clientId, redirectUri);
        //    var store = GetRefreshTokenStore();
        //    var relevantTokens = await store.GetRefreshTokens(clientId, redirectUri, DateTimeOffset.UtcNow);

        //    var validationResult = await this.TokenProvider.ValidateRefreshToken(relevantTokens, refreshToken);

        //    if (validationResult.IsValidated)
        //    {
        //        if (validationResult.Entity.ClientId == clientId && validationResult.Entity.RedirectUri == redirectUri)
        //        {
        //            this.logger.DebugFormat("Refresh token is valid. It belongs to the user '{0}', client '{1}' and redirect uri '{2}'", validationResult.Entity.Subject, validationResult.Entity.ClientId, validationResult.Entity.RedirectUri);

        //            // Delete refresh token to prevent it being used again
        //            var deleteResult = await store.DeleteRefreshToken(validationResult.Entity);

        //            if (!deleteResult)
        //            {
        //                this.logger.ErrorFormat("Unable to delete used refresh token: {0}", JsonConvert.SerializeObject(validationResult.Entity));
        //            }



        //            var identity = new ClaimsIdentity(new Claim[]{
        //            new Claim(Constants.ClaimType.Client,validationResult.Entity.ClientId),
        //            new Claim(ClaimTypes.Name, validationResult.Entity.Subject),
        //                new Claim(Constants.ClaimType.RedirectUri, validationResult.Entity.RedirectUri)
        //            }, Constants.AuthenticationType);

        //            if (validationResult.Entity.Scope != null)
        //            {
        //                identity.AddClaim(new Claim(Constants.ClaimType.Scope, string.Join(" ", validationResult.Entity.Scope)));
        //            }

        //            return identity;
        //        }
        //    }

        //    this.logger.WarnFormat("Refresh token '{0}' is not valid", refreshToken);

        //    return new ClaimsIdentity();
        //}



        //public async Task<TokenCreationResult<IAccessToken>> CreateAccessTokenAsync(ClaimsIdentity userIdentity, TimeSpan expire, string clientId, string redirectUri, IEnumerable<string> scope)
        //{
        //    if (!userIdentity.IsAuthenticated)
        //    {
        //        this.logger.Error("The specified identity is not authenticated");

        //        return null;
        //    }
        //    var store = GetAccessTokenStore();
        //    // Delete all expired access tokens
        //    await store.DeleteAccessTokens(DateTimeOffset.UtcNow);

        //    // Add scope claims
        //    if (scope != null)
        //    {
        //        userIdentity.AddClaims(scope.Select(x => new Claim(Constants.ClaimType.Scope, x)));
        //    }

        //    this.logger.DebugFormat("Creating access token for client '{0}', redirect uri '{1}' and user '{2}'", clientId, redirectUri, userIdentity.Name);

        //    var createResult = await this.TokenProvider.CreateAccessToken(
        //        clientId,
        //        redirectUri,
        //        userIdentity,
        //        scope,
        //        DateTimeOffset.UtcNow.Add(expire));

        //    // Add access token to database
        //    var insertResult = await store.InsertAccessToken(createResult.Entity);

        //    if (insertResult != null)
        //    {
        //        this.logger.Debug("Successfully created and stored access token");

        //        return new TokenCreationResult<IAccessToken>(createResult.Token, insertResult);
        //    }

        //    this.logger.Error("Unable to create and/or store access token");

        //    return null;
        //}
        //public async Task<TokenCreationResult<IRefreshToken>> CreateRefreshTokenAsync(ClaimsIdentity userIdentity, TimeSpan expire, string clientId, string redirectUri, IEnumerable<string> scope)
        //{
        //    if (!userIdentity.IsAuthenticated)
        //    {
        //        this.logger.Error("The specified principal is not authenticated");

        //        return null;
        //    }
        //    var store = GetRefreshTokenStore();

        //    // Delete all expired refresh tokens
        //    await store.DeleteRefreshTokens(DateTimeOffset.UtcNow);

        //    // Add scope claims
        //    if (scope != null)
        //    {
        //        userIdentity.AddClaims(scope.Select(x => new Claim(Constants.ClaimType.Scope, x)).ToArray());
        //    }

        //    this.logger.DebugFormat("Creating refresh token for client '{0}', redirect uri '{1}' and user '{2}'", clientId, redirectUri, userIdentity.Name);

        //    var createResult =
        //        await
        //        this.TokenProvider.CreateRefreshToken(
        //            clientId,
        //            redirectUri,
        //            userIdentity,
        //            scope,
        //            DateTimeOffset.UtcNow.Add(expire));

        //    // Add refresh token to database
        //    var insertResult = await store.InsertRefreshToken(createResult.Entity);

        //    if (insertResult != null)
        //    {
        //        this.logger.Debug("Successfully created and stored refresh token");

        //        return new TokenCreationResult<IRefreshToken>(createResult.Token, insertResult);
        //    }

        //    this.logger.Error("Unable to create and/or store refresh token");

        //    return null;
        //}


        //public async Task<TokenCreationResult<IAuthorizationCode>> CreateAuthorizationCodeAsync(IAuthorizationCode authorizationCode)
        //{
        //    if (!userIdentity.IsAuthenticated)
        //    {
        //        this.logger.Error("The specified user is not authenticated");

        //        return null;
        //    }

        //    var client = userIdentity.Claims.FirstOrDefault(x => x.Type == Constants.ClaimType.Client);

        //    if (client == null || string.IsNullOrEmpty(client.Value))
        //    {
        //        throw new ArgumentException("The specified principal does not have a valid client claim type", "userIdentity");
        //    }
        //    var store = GetAuthorizationCodeStore();
        //    // Delete all expired authorization codes
        //    await store.DeleteAuthorizationCodes(DateTimeOffset.UtcNow);

        //    // Add scope claims
        //    if (scope != null)
        //    {
        //        userIdentity.AddClaims(scope.Select(x => new Claim(Constants.ClaimType.Scope, x)));
        //    }

        //    // Create and store authorization code for future use
        //    this.logger.DebugFormat("Creating authorization code for client '{0}', redirect uri '{1}' and user '{2}'", client, redirectUri, userIdentity.Name);

        //    //var createResult = await this.TokenProvider.CreateAuthorizationCode(
        //    //    client.Value,
        //    //    redirectUri,
        //    //    userIdentity,
        //    //    scope,
        //    //    DateTimeOffset.UtcNow.Add(expire));

        //    // Add authorization code to database
        //    var insertResult = await store.InsertAuthorizationCode(createResult.Entity);

        //    if (insertResult != null)
        //    {
        //        this.logger.Debug("Successfully created and stored authorization code");

        //        return new TokenCreationResult<IAuthorizationCode>(createResult.Token, insertResult);
        //    }

        //    this.logger.Error("Unable to create and/or store authorization code");

        //    return null;
        //}



        public async Task<IAuthorizationCode> FindAuthorizationCodeAsync(string code)
        {
            var store = GetAuthorizationCodeStore();
            var relevantCodes = await store.GetAuthorizationCode(code);

            return relevantCodes;
        }

        public async Task<IAuthorizationCode> CreateAuthorizationCodeAsync(IAuthorizationCode code)
        {
            var store = GetAuthorizationCodeStore();
            // Delete all expired authorization codes
            await store.DeleteAuthorizationCodes(DateTimeOffset.UtcNow);


            // Add authorization code to database
            var insertResult = await store.InsertAuthorizationCode(code);

            return insertResult;
        }

        public async Task<bool> DeleteAuthorizationCode(IAuthorizationCode code)
        {
            var store = GetAuthorizationCodeStore();
            return await store.DeleteAuthorizationCode(code);
        }

        public async Task<IAccessToken> FindAccessTokenAsync(string accessToken)
        {
            var store = GetAccessTokenStore();
            var token = await store.GetAccessToken(accessToken);

            return token;
        }

        public async Task<bool> DeleteAccessTokenAsync(IAccessToken accessToken)
        {
            var store = GetAccessTokenStore();
            return await store.DeleteAccessToken(accessToken);
        }

        public async Task<IAccessToken> CreateAccessTokenAsync(IAccessToken accessToken)
        {
            var store = GetAccessTokenStore();

            // Delete all expired accessTokens
            await store.DeleteAccessTokens(DateTimeOffset.UtcNow);


            // Add accessToken to database
            var insertResult = await store.InsertAccessToken(accessToken);

            return insertResult;
        }

        public async Task<IRefreshToken> FindRefreshTokenAsync(string refreshToken)
        {
            var store = GetRefreshTokenStore();
            var token = await store.GetRefreshToken(refreshToken);

            return token;
        }

        public async Task<bool> DeleteRefreshTokenAsync(IRefreshToken refreshToken)
        {
            var store = GetRefreshTokenStore();
           return await store.DeleteRefreshToken(refreshToken);

        }

        public async Task<IRefreshToken> CreateRefreshTokenAsync(IRefreshToken refreshToken)
        {
            var store = GetRefreshTokenStore();
            // Delete all expired refreshTokens
            await store.DeleteRefreshTokens(DateTimeOffset.UtcNow);


            // Add refreshToken to database
            var insertResult = await store.InsertRefreshToken(refreshToken);

            return insertResult;
        }
    }
}
