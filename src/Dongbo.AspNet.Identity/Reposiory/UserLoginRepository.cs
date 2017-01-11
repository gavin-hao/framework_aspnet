using Dongbo.Data;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Reposiory
{
    internal class UserLoginRepository<TUser> : RepositoryBase where TUser : IdentityUser
    {

        internal async Task Insert(TUser user, UserLoginInfo login)
        {
            var url = string.Format("api/UserLogin/create/{0}", user.Id);
            var result =await ServiceClient.PostAsync<bool, UserLoginInfo>(url, login);

        }
        /// <summary>
        /// Return a userId given a user's login
        /// </summary>
        /// <param name="userLogin">The user's login info</param>
        /// <returns></returns>
        internal async Task<TUser> FindUserByLogin(UserLoginInfo login)
        {
            var url = string.Format("api/User?provider={0}&providerKey={1}", login.LoginProvider, login.ProviderKey);
            var result = await ServiceClient.GetAsync<TUser>(url);

            return result;
        }

        /// <summary>
        /// Returns a list of user's logins
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        internal async Task<List<UserLoginInfo>> FindByUserId(string userId)
        {
            var url = string.Format("api/UserLogin?uid={0}", userId);
            var result = await ServiceClient.GetAsync<List<UserLoginInfo>>(url);

            return result;

        }

        internal async Task Delete(TUser user, UserLoginInfo login)
        {
            var url = string.Format("api/UserLogin?userId={0}&provider={1}&providerKey={2}", user.Id, login.LoginProvider, login.ProviderKey);
            var result =await ServiceClient.DeleteAsync<bool>(url);
        }
        /// <summary>
        /// Deletes all Logins from a user in the UserLogins table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        internal async Task DeleteByUserId(string userId)
        {
            var url = string.Format("api/UserLogin?userId={0}", userId);
            var result =await ServiceClient.DeleteAsync<int>(url);
        }
    }
}
