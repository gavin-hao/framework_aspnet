using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    /// <summary>
    ///  defined an interface find user by phonenumber
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IUserPhoneNumberExtraStore<TUser, in TKey> : IUserStore<TUser, TKey> where TUser : class, IUser<TKey>
    {
        /// <summary>
        ///     Returns the user associated with this phonenumber
        /// </summary>
        /// <param name="phoneNumber">phoneNumber</param>
        /// <returns></returns>
        Task<TUser> FindByPhoneNumberAsync(string phoneNumber);
    }
}
