using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Managers
{
    public class AspNetIdentityUserManager<TUser> : IUserManager where TUser : class, IUser<string>
    {
        public AspNetIdentityUserManager(UserManager<TUser> manager)
        {
            this.UserManager = manager;
        }
        public UserManager<TUser> UserManager { get; set; }
        /// <summary>
        /// Authenticates the user using username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async virtual Task<ClaimsIdentity> AuthenticateUserWithPasswordAsync(string username, string password)
        {
            bool shouldLockout = true;
            var user = await UserManager.FindByNameAsync(username);
            if (user != null)
            {
                if (await UserManager.IsLockedOutAsync(user.Id))
                    return Anonymous;
            }
            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return Anonymous;
            }
            if (await UserManager.CheckPasswordAsync(user, password))
            {
                await UserManager.ResetAccessFailedCountAsync(user.Id);
                var identity = await UserManager.CreateIdentityAsync(user, Constants.AuthenticationType);
                return identity;
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return Anonymous;
                }
            }


            return Anonymous;
        }
        private ClaimsIdentity Anonymous
        {
            get { return new ClaimsIdentity(); }
        }
        /// <summary>
        ///  Authenticates the user using username only. This method is used to get new user claims after
        /// a refresh token has been used. You can therefore assume that the user is already logged in.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async virtual Task<ClaimsIdentity> AuthenticateUserAsync(string username)
        {
            var user = await UserManager.FindByNameAsync(username);

            if (user != null)
            {
                var identity = await UserManager.CreateIdentityAsync(user, Constants.AuthenticationType);
                return identity;
            }

            return Anonymous;
        }
    }
}
