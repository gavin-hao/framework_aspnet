using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Threading;
using Dongbo.AspNet.Identity.Reposiory;
using System.Security.Claims;
using Dongbo.Common.Util;
using Dongbo.AspNet.Identity.Extension;

namespace Dongbo.AspNet.Identity
{

    public class UserStore : UserStore<IdentityUser>
    {
        public UserStore() : base() { }
    }
    public class UserStore<TUser> : UserStore<TUser, IdentityRole> where TUser : IdentityUser, new()
    {
        public UserStore() : base() { }
    }
    public class UserStore<TUser, TRole> : IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser, string>,
        IUserPhoneNumberStore<TUser>,
        IUserPhoneNumberExtraStore<TUser, string>,
        IUserNicknameExtraStore<TUser,string>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser, string>
        where TUser : IdentityUser, new()
        where TRole : IdentityRole<string>, new()
    {
        private UserRepository<TUser> userRepository;
        private UserClaimRepository userClaimRepository;
        private UserLoginRepository<TUser> userLoginRepository;
        public UserStore()
        {
            userRepository = new UserRepository<TUser>();
            userClaimRepository = new UserClaimRepository();
            userLoginRepository = new UserLoginRepository<TUser>();
        }

        #region IUserStore
        /// <summary>
        /// Insert a new TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(user.From))
                user.From = "local";
            await userRepository.Insert(user);

            //return Task.FromResult<object>(null);
        }

        public async Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">TUser.id </param>
        /// <returns></returns>
        public async Task<TUser> FindByIdAsync(string userId)
        {
            TUser result = await userRepository.GetUserById(userId) as TUser;
            if (result != null)
            {
                return result;
            }

            return null;
        }
        /// <summary>
        /// supports email and phone number 
        /// </summary>
        /// <param name="userName">email or phone</param>
        /// <returns></returns>
        public async Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }
            List<TUser> result = null;
            //var emailLogin = StringHelper.IsEmail(userName);
            //if (!emailLogin)
            //{
            //    result = userRepository.GetUserByPhone(userName);
            //}
            //else
            //{
            //    result = userRepository.GetUserByEmail(userName);
            //}
            result = await userRepository.GetUserByName(userName);
            // Should I throw if > 1 user?
            if (result != null && result.Count == 1)
            {
                return result[0];
            }

            return null;
        }

        public async Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.Version == 0)//
            {
                user.Version = 1;
            }
            await userRepository.Update(user);

        }
        public void Dispose()
        {
            userRepository = null;
            userClaimRepository = null;
            userLoginRepository = null;
        }
        #endregion

        #region IUserLoginStore
        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            await userLoginRepository.Insert(user, login);

        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            TUser user = await userLoginRepository.FindUserByLogin(login);
            //if (userId != "0")
            //{
            //    TUser user = await userRepository.GetUserById(userId);
            //    if (user != null)
            //    {
            //        return user;
            //    }
            //}

            return user;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            List<UserLoginInfo> userLogins = new List<UserLoginInfo>();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<UserLoginInfo> logins = await userLoginRepository.FindByUserId(user.Id);
            if (logins != null)
            {
                return logins;
            }

            return null;
        }

        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            await userLoginRepository.Delete(user, login);

        }



        #endregion

        #region IUserPasswordStore

        public async Task<string> GetPasswordHashAsync(TUser user)
        {

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return user.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(TUser user)
        {
            return user.PasswordHash != null;
        }

        public async Task SetPasswordHashAsync(TUser user, string passwordHash)
        {

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
        }

        #endregion

        #region IUserSecurityStampStore
        public async Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return user.SecurityStamp;
        }

        public async Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
        }
        #endregion

        #region IUserEmailStore
        public async Task<TUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(" email");
            }

            List<TUser> result = await userRepository.GetUserByEmail(email);

            // Should I throw if > 1 user?
            if (result != null && result.Count >= 1)
            {
                return result[0];
            }

            return null;
        }

        public async Task<string> GetEmailAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return user.Email;

        }

        public async Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return user.EmailConfirmed;
        }

        public async Task SetEmailAsync(TUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.Email = email;
        }

        public async Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
        }
        #endregion

        #region IUserTwoFactorStore
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }
        #endregion

        #region IUserLockoutStore
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return
                Task.FromResult(user.LockoutEnd.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEnd.Value, DateTimeKind.Utc))
                    : DateTimeOffset.MinValue);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEnd = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }
        #endregion

        #region IUserClaimStore
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("user");
            }

            //userClaimRepository.Insert(claim, user.Id);
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            user.Claims.Add(new IdentityUserClaim<string> { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            return Task.FromResult<object>(null);
        }
        /// <summary>
        /// Returns all claims for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            //todo: how to manage  user claims??
            //EnsureClaimsLoaded(user);
            var claims = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();

            // ClaimsIdentity identity = userClaimRepository.FindByUserId(user.Id);

            return Task.FromResult<IList<Claim>>(claims);
        }

        private async Task EnsureClaimsLoaded(TUser user)
        {
            if (!user.ClaimsLoaded)
            {
                var claims = await userClaimRepository.FindByUserId(user.Id);
                user.ClaimsLoaded = true;
            }
        }

        /// <summary>
        /// Removes a claim froma user
        /// </summary>
        /// <param name="user">User to have claim removed</param>
        /// <param name="claim">Claim to be removed</param>
        /// <returns></returns>
        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            //var removes=
            user.Claims.RemoveAll(p => p.UserId == user.Id && p.ClaimType == claim.Type && p.ClaimValue == claim.Value);
            //userClaimRepository.Delete(user, claim);

            return Task.FromResult<object>(null);
        }

        #endregion

        public IQueryable<TUser> Users
        {
            get { throw new NotImplementedException(); }
        }


        #region RoleStore
        public Task AddToRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var roles = await userRepository.FindRoleByUserId(user.Id);
            return roles;

        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("role");
            }

            List<string> roles = await userRepository.FindRoleByUserId(user.Id);
            {
                if (roles != null && roles.Contains(roleName))
                {
                    return true;
                }
            }

            return false;
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PhoneNumber
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentNullException("phoneNumber");
            }
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }
        #endregion

        public async Task<TUser> FindByPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentNullException(" phoneNumber");
            }

            List<TUser> result = await userRepository.GetUserByPhone(phoneNumber);

            // Should I throw if > 1 user?
            if (result != null && result.Count >= 1)
            {
                return result[0];
            }

            return null;
        }

        public async Task<TUser> FindByNicknameAsync(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
            {
                throw new ArgumentNullException("nickname");
            }

            List<TUser> result = await userRepository.GetUserByNickname(nickname);

            // Should I throw if > 1 user?
            if (result != null && result.Count >= 1)
            {
                return result[0];
            }

            return null;
        }

      
        public Task<string> GetNicknameAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Nickname);
        }
    }
}
