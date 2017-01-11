using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using System.Security.Claims;
namespace Dongbo.AspNet.Identity.Extension
{
    public class ApplicationSignInManager<TUser> : IDisposable where TUser : class, IUser<string>
    {
        public ApplicationSignInManager(ApplicationUserManager<TUser> userManager, IAuthenticationManager authenticationManager)
        {
            this.UserManager = userManager;
            this.AuthenticationManager = authenticationManager;
            this.RequireValidEmail = true;
            ShouldLockout = true;
        }
        public string AuthenticationType
        {
            get { return _authType ?? DefaultAuthenticationTypes.ApplicationCookie; }
            set { _authType = value; }
        }private string _authType;



        /// <summary>
        ///  ApplicationUserManager
        /// </summary>
        public ApplicationUserManager<TUser> UserManager { get; set; }

        /// <summary>
        /// Used to sign in identities
        /// </summary>
        public IAuthenticationManager AuthenticationManager { get; set; }
        /// <summary>
        /// requires email been confirmed
        /// </summary>
        public bool RequireValidEmail { get; set; }

        /// <summary>
        /// should lockout account ?
        /// </summary>
        public bool ShouldLockout { get; set; }

        /// <summary>
        /// add your claims  like nickname avatar etc..
        /// </summary>
        public Action<ClaimsIdentity, TUser> OnCreatedUserIdentity;
        /// <summary>
        /// Called to generate the ClaimsIdentity for the user, override to add additional claims before SignIn
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ClaimsIdentity> CreateUserIdentityAsync(TUser user)
        {
            var identity = await UserManager.CreateIdentityAsync(user, AuthenticationType);
            if (OnCreatedUserIdentity != null)
            {
                OnCreatedUserIdentity(identity, user);
            }
            return identity;
        }

        public async Task<SignInStatusCode> PasswordSignInAsync(string userName, string password, bool isPersistent)
        {
            var shouldLockout = ShouldLockout;
            return await PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }
        /// <summary>
        /// Send a two factor code to a user
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual async Task<bool> SendTwoFactorCodeAsync(string provider)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return false;
            }

            var token = await UserManager.GenerateTwoFactorTokenAsync(userId, provider);
            // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
            await UserManager.NotifyTwoFactorTokenAsync(userId, provider, token);
            return true;
        }
        /// <summary>
        /// Creates a user identity and then signs the identity using the AuthenticationManager
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isPersistent"></param>
        /// <param name="rememberBrowser"></param>
        /// <returns></returns>
        public virtual async Task SignInAsync(TUser user, bool isPersistent, bool rememberBrowser, bool signoutPartialCookies = true)
        {
            var userIdentity = await CreateUserIdentityAsync(user);
            // Clear any partial cookies from external or two factor partial sign ins
            //var authTypes = Constants.DefaultAuthenticationTypes.Union(new String[] { DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie });
            if (signoutPartialCookies)
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            if (rememberBrowser)
            {
                var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(user.Id);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity, rememberBrowserIdentity);
            }
            else
            {
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity);
            }
        }
        public virtual async Task SignInAsync(TUser user, bool isPersistent)
        {
            await SignInAsync(user, isPersistent, false);
        }

        /// <summary>
        /// Signs the current user out of the application.
        /// </summary>
        public virtual async Task SignOutAsync()
        {
            AuthenticationManager.SignOut(this.AuthenticationType, DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);
        }
        /// <summary>
        /// Get the user id that has been verified already or null.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetVerifiedUserIdAsync()
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorCookie);
            if (result != null && result.Identity != null && !String.IsNullOrEmpty(result.Identity.GetUserId()))
            {
                return result.Identity.GetUserId();
            }
            return "";
        }
        /// <summary>
        /// override base method to use ApplicationUserManager methods
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="shouldLockout"></param>
        /// <returns></returns>
        public async Task<SignInStatusCode> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            if (UserManager == null)
            {
                return SignInStatusCode.Failure;
            }
            TUser user = null;
            if (userName.IsEmail())
            {
                user = await UserManager.FindByEmailAsync(userName);

            }
            else if (userName.IsMobilePhone())
            {
                user = await UserManager.FindByPhoneNumberAsync(userName);
            }
            else
            {
                user = await UserManager.FindByNameAsync(userName);

            }
            if (user == null)
            {
                return SignInStatusCode.Failure;
            }
            if (RequireValidEmail && userName.IsEmail())
            {
                var valid = await UserManager.IsEmailConfirmedAsync(user);
                if (!valid)
                {
                    return SignInStatusCode.InvalidEmail;
                }
            }
            if (await UserManager.IsLockedOutAsync(user))
            {
                return SignInStatusCode.LockedOut;
            }
            if (await UserManager.CheckPasswordAsync(user, password))
            {
                await UserManager.ResetAccessFailedCountAsync(user.Id);
                return await SignInOrTwoFactor(user, isPersistent);
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatusCode.LockedOut;
                }
            }
            return SignInStatusCode.Failure;
        }
        public async Task<SignInResult<TUser>> PasswordSignInAsync2(string userName, string password)
        {
            var shouldLockout = ShouldLockout;
            var result = new SignInResult<TUser>() { User = null, StatusCode = SignInStatusCode.Failure };
            if (UserManager == null)
            {

                return result;
            }
            TUser user = null;
            if (userName.IsEmail())
            {
                user = await UserManager.FindByEmailAsync(userName);

            }
            else if (userName.IsMobilePhone())
            {
                user = await UserManager.FindByPhoneNumberAsync(userName);
            }
            else
            {
                user = await UserManager.FindByNameAsync(userName);

            }
            if (user == null)
            {

                return result;
            }
            result.User = user;
            if (RequireValidEmail && userName.IsEmail())
            {
                var valid = await UserManager.IsEmailConfirmedAsync(user);
                if (!valid)
                {
                    result.StatusCode = SignInStatusCode.InvalidEmail;
                    return result;
                }
            }
            if (await UserManager.IsLockedOutAsync(user))
            {

                result.StatusCode = SignInStatusCode.LockedOut;
                return result;
            }
            if (await UserManager.CheckPasswordAsync(user, password))
            {
                await UserManager.ResetAccessFailedCountAsync(user.Id);

                var state = await SignInPrimaryOrTwoFactor(user, false);
                result.StatusCode = state;
                return result;
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    result.StatusCode = SignInStatusCode.LockedOut;
                    return result;
                }
            }
            result.StatusCode = SignInStatusCode.Failure;
            return result;
        }
        /// <summary>
        /// Sign the user in using an associated external login
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public async Task<SignInStatusCode> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user == null)
            {
                return SignInStatusCode.Failure;
            }
            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatusCode.LockedOut;
            }
            return await SignInOrTwoFactor(user, isPersistent);
        }
        private async Task<SignInStatusCode> SignInOrTwoFactor(TUser user, bool isPersistent)
        {
            var id = Convert.ToString(user.Id);
            if (await UserManager.GetTwoFactorEnabledAsync(user)
                && (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0
                && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id))
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                AuthenticationManager.SignIn(identity);
                return SignInStatusCode.RequiresVerification;
            }
            await SignInAsync(user, isPersistent, false);
            return SignInStatusCode.Success;
        }
        private async Task<SignInStatusCode> SignInPrimaryOrTwoFactor(TUser user, bool isPersistent)
        {
            var id = Convert.ToString(user.Id);
            if (await UserManager.GetTwoFactorEnabledAsync(user)
                && (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0
                && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id))
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                AuthenticationManager.SignIn(identity);
                return SignInStatusCode.RequiresVerification;
            }
            await SignInAsync(user, isPersistent, false, false);
            return SignInStatusCode.Success;
        }
        /// <summary>
        /// Has the user been verified (ie either via password or external login)
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasBeenVerifiedAsync()
        {
            return await GetVerifiedUserIdAsync() != null;
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }

}
