using Dongbo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Reposiory
{
    internal class UserClaimRepository : RepositoryBase
    {
        public UserClaimRepository(string serviceName = "DongboUser")
            : base(serviceName)
        {

        }
        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim">User's claim to be added</param>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        internal async Task Insert(Claim claim, string userId)
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        internal async Task<ICollection<Claim>> FindByUserId(string userId)
        {
            return new List<Claim>();
            //string url = string.Format("api/UserClaim?uid={0}", userId);
            //var userClaims = ServiceClient.GetAsync<List<IdentityUserClaim<string>>>(url).Result;
            //if (userClaims == null || userClaims.Count == 0)
            //{
            //    return new List<Claim>();
            //}
            //return userClaims.Select(p => new Claim(p.ClaimType, p.ClaimValue)).ToList();
        }
        /// <summary>
        /// Deletes a claim from a user 
        /// </summary>
        /// <param name="user">The user to have a claim deleted</param>
        /// <param name="claim">A claim to be deleted from user</param>
        /// <returns></returns>
        internal async Task Delete(IdentityUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        internal async Task DeleteByUserId(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
