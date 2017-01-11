using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    public class ApplicationUserManager<TUser> : UserManager<TUser> where TUser : class, IUser<string>
    {
        protected const string ResetPasswordTokenPurpose = "ResetPassword";
        protected const string ConfirmEmailTokenPurpose = "Confirmation";

        private bool _disposed = false;
        public ApplicationUserManager(IUserStore<TUser> store)
            : base(store)
        {
            //this.EmailService = new EmailService();
            //this.SmsService = new SmsService();
            //this.PasswordHasher = new DongboSystemPasswordHasher();
        }
        public virtual bool SupportsUserPhoneNumberExtra
        {
            get
            {
                ThrowIfDisposed();
                return Store is IUserPhoneNumberExtraStore<TUser, string>;
            }
        }
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     When disposing, actually dipose the store
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Store.Dispose();
                _disposed = true;
            }
        }
        internal IUserEmailStore<TUser, string> GetEmailStore()
        {
            var cast = Store as IUserEmailStore<TUser, string>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotIUserEmailStore");
            }
            return cast;
        }
        internal IUserLockoutStore<TUser, string> GetUserLockoutStore()
        {
            var cast = Store as IUserLockoutStore<TUser, string>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotIUserLockoutStore");
            }
            return cast;
        }
        internal IUserSecurityStampStore<TUser, string> GetSecurityStore()
        {
            var cast = Store as IUserSecurityStampStore<TUser, string>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotIUserSecurityStampStore");
            }
            return cast;
        }
        internal IUserPhoneNumberStore<TUser, string> GetPhoneNumberStore()
        {
            var cast = Store as IUserPhoneNumberStore<TUser, string>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotIUserPhoneNumberStore");
            }
            return cast;
        }
        internal IUserPhoneNumberExtraStore<TUser, string> GetPhoneNumberExtraStore()
        {
            var cast = Store as IUserPhoneNumberExtraStore<TUser, string>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotIUserPhoneNumberExtraStore");
            }
            return cast;
        }
        internal IUserNicknameExtraStore<TUser, string> GetNicknameExtraStore()
        {
            var cast = Store as IUserNicknameExtraStore<TUser, string>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotINicknameExtraStore");
            }
            return cast;
        }
        internal IUserTwoFactorStore<TUser, string> GetUserTwoFactorStore()
        {
            var cast = Store as IUserTwoFactorStore<TUser, string>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotIUserTwoFactorStore");
            }
            return cast;
        }
        internal IUserClaimStore<TUser, string> GetClaimStore()
        {
            var cast = Store as IUserClaimStore<TUser, string>;
            if (cast == null)
            {
                throw new NotSupportedException("StoreNotIUserClaimStore");
            }
            return cast;
        }


        //public override async Task<IdentityResult> CreateAsync(TUser user, string password)
        //{
        //    await UpdateSecurityStampInternal(user);
        //    var result = await UserValidator.ValidateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        return result;
        //    }
        //    if (UserLockoutEnabledByDefault && SupportsUserLockout)
        //    {
        //        await GetUserLockoutStore().SetLockoutEnabledAsync(user, true);
        //    }
        //    await Store.CreateAsync(user);
        //    return IdentityResult.Success;
        //}
        public override async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var store = GetEmailStore();
            var user = await FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException(string.Format("user[{0}] not found", userId));
            }
            if (!await VerifyUserTokenAsync(user, ConfirmEmailTokenPurpose, token))
            {
                return IdentityResult.Failed("Invalid Token");
            }
            await store.SetEmailConfirmedAsync(user, true);
            return await UpdateAsync(user);
        }

        public override async Task<IdentityResult> ResetAccessFailedCountAsync(string userId)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();
            var user = await FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User Id: {0} Not Found", userId));
            }

            if (await GetAccessFailedCountAsync(user) == 0)
            {
                return IdentityResult.Success;
            }

            await store.ResetAccessFailedCountAsync(user);
            return await UpdateAsync(user);
        }
        #region new extend methods
        /// <summary>
        ///find user by email/phone/username
        /// </summary>
        /// <param name="userName"> username may be a email /phonenumber /username</param>
        /// <returns></returns>
        public async Task<TUser> FindAsync(string userName)
        {
            ThrowIfDisposed();
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            if (userName.IsEmail())
            {
                return await GetEmailStore().FindByEmailAsync(userName);
            }
            else if (userName.IsMobilePhone())
            {
                return await GetPhoneNumberExtraStore().FindByPhoneNumberAsync(userName);
            }
            return await Store.FindByNameAsync(userName);
        }
        public virtual async Task<TUser> FindByNicknameAsync(string nickname)
        {
            ThrowIfDisposed();
            var store = GetNicknameExtraStore();
            if (nickname == null)
            {
                throw new ArgumentNullException("phoneNumber");
            }
            return await store.FindByNicknameAsync(nickname);
        }
        public virtual async Task<TUser> FindByPhoneNumberAsync(string phoneNumber)
        {
            ThrowIfDisposed();
            var store = GetPhoneNumberExtraStore();
            if (phoneNumber == null)
            {
                throw new ArgumentNullException("phoneNumber");
            }
            return await store.FindByPhoneNumberAsync(phoneNumber);
        }
        public virtual async Task<IdentityResult> SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var store = GetPhoneNumberStore();

            await store.SetPhoneNumberAsync(user, phoneNumber);
            await store.SetPhoneNumberConfirmedAsync(user, false);
            await UpdateSecurityStampInternal(user);
            return await UpdateAsync(user);
        }
        public virtual async Task<IdentityResult> SetEmailWithConfirmedAsync(string userId, string email)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();
            var user = await FindByIdAsync(userId);
            if (user == null)
            {
                if (user == null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User Id: {0} Not Found", userId));
                }
            }
            await store.SetEmailAsync(user, email);
            await store.SetEmailConfirmedAsync(user, true);
            await UpdateSecurityStampInternal(user);
            return await UpdateAsync(user);
        }
        public virtual async Task<bool> IsLockedOutAsync(TUser user)
        {
            var store = GetUserLockoutStore();
            if (!await store.GetLockoutEnabledAsync(user))
            {
                return false;
            }
            var lockoutTime = await store.GetLockoutEndDateAsync(user);
            return lockoutTime >= DateTimeOffset.UtcNow;
        }

        public virtual async Task<IdentityResult> AccessFailedAsync(TUser user)
        {
            ThrowIfDisposed();
            var store = GetUserLockoutStore();

            // If this puts the user over the threshold for lockout, lock them out and reset the access failed count
            var count = await store.IncrementAccessFailedCountAsync(user);
            if (count >= MaxFailedAccessAttemptsBeforeLockout)
            {
                await
                    store.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.Add(DefaultAccountLockoutTimeSpan));

                await store.ResetAccessFailedCountAsync(user);
            }
            return await UpdateAsync(user);
        }
        public virtual async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var store = GetUserLockoutStore();

            return await store.GetAccessFailedCountAsync(user);
        }
        public virtual async Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var store = GetUserTwoFactorStore();

            return await store.GetTwoFactorEnabledAsync(user);
        }

        public virtual async Task<IList<string>> GetValidTwoFactorProvidersAsync(TUser user)
        {
            ThrowIfDisposed();

            var results = new List<string>();
            foreach (var f in TwoFactorProviders)
            {
                if (await f.Value.IsValidProviderForUserAsync(this, user))
                {
                    results.Add(f.Key);
                }
            }
            return results;
        }

        public virtual async Task<bool> VerifyPasswordResetTokenAsync(TUser user, string token)
        {
           
            var purpose = ResetPasswordTokenPurpose;// "ResetPassword";
            // Make sure the token is valid
            return await VerifyUserTokenAsync(user, purpose, token);
        }
        public virtual async Task<bool> VerifyUserTokenAsync(TUser user, string purpose, string token)
        {
            if (UserTokenProvider == null)
            {
                throw new NotSupportedException("No Token Provider");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            // Make sure the token is valid
            return await UserTokenProvider.ValidateAsync(purpose, token, this, user);
        }
        public virtual async Task<string> GenerateUserTokenAsync(string purpose, TUser user)
        {
            if (UserTokenProvider == null)
            {
                throw new NotSupportedException("No Token Provider");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return await UserTokenProvider.GenerateAsync(purpose, this, user);
        }

        public virtual async Task<bool> VerifyTwoFactorTokenAsync(TUser user, string twoFactorProvider, string token)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (!TwoFactorProviders.ContainsKey(twoFactorProvider))
            {
                throw new NotSupportedException("No TwoFactorProvider");
            }
            return await TwoFactorProviders[twoFactorProvider].ValidateAsync(twoFactorProvider, token, this, user);
        }
        public virtual async Task<string> GenerateTwoFactorTokenAsync(TUser user, string twoFactorProvider)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (!TwoFactorProviders.ContainsKey(twoFactorProvider))
            {
                throw new NotSupportedException("No TwoFactorProvider");
            }

            return await TwoFactorProviders[twoFactorProvider].GenerateAsync(twoFactorProvider, this, user);
        }
        public virtual async Task<bool> IsEmailConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var store = GetEmailStore();

            return await store.GetEmailConfirmedAsync(user);
        }

        public virtual async Task<string> GetEmailAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var store = GetEmailStore();

            return await store.GetEmailAsync(user);
        }

        public virtual async Task AddClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            var claimStore = GetClaimStore();
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await claimStore.AddClaimAsync(user, claim);
            return;
        }
        #endregion
        public async Task UpdateSecurityStampInternal(TUser user)
        {
            if (SupportsUserSecurityStamp)
            {
                await GetSecurityStore().SetSecurityStampAsync(user, NewSecurityStamp());
            }
        }

        private static string NewSecurityStamp()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
