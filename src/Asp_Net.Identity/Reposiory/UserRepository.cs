using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dongbo.Data;
using System.Globalization;
using System.Threading;
namespace Dongbo.AspNet.Identity.Reposiory
{
    
    internal class UserRepository<TUser> : RepositoryBase where TUser : IdentityUser, new()
    {
        public UserRepository(string serviceName = "DongboUser")
            : base(serviceName)
        {
        }
        public async Task Insert(TUser user)
        {
            user.CreateTime = DateTime.Now;
            var url = "api/user/create";
            var success = await ServiceClient.PostAsync<bool, TUser>(url, user);

        }

        internal async Task<List<TUser>> GetUserByName(string userName)
        {
            var url = string.Format("api/user?name={0}", userName);
            var result = await ServiceClient.GetAsync<List<TUser>>(url);

            return result;
        }

        internal async Task<List<TUser>> GetUserByEmail(string email)
        {
            var url = string.Format("api/user?email={0}", email);

            var result = await ServiceClient.GetAsync<List<TUser>>(url);

            return result;

        }


        internal async Task<List<TUser>> GetUserByNickname(string nickname)
        {
            var url = string.Format("api/user?nickname={0}", nickname);
            var result = await ServiceClient.GetAsync<List<TUser>>(url);

            return result;
        }
        internal async Task<List<TUser>> GetUserByPhone(string phone)
        {
            var url = string.Format("api/user?phone={0}", phone);
            var result =await ServiceClient.GetAsync<List<TUser>>(url);

            return result;
        }
        internal async Task Update(TUser user)
        {
            //var oldUser = GetUserById(user.Id);
            //if (oldUser == null || string.IsNullOrEmpty(oldUser.UserName))
            //    return;
            var url = "api/user/update";
            var result = await ServiceClient.PostAsync<bool, TUser>(url, user);

        }

        internal async Task<TUser> GetUserById(string userId)
        {
            var url = string.Format("api/user/{0}", userId);
            var result = await ServiceClient.GetAsync<TUser>(url);

            return result;
        }

        internal async Task<List<string>> FindRoleByUserId(string userId)
        {
            var url = string.Format("api/user/role?uid={0}", userId);
            var result = await ServiceClient.GetAsync<List<string>>(url);

            return result;
        }
    }
}
