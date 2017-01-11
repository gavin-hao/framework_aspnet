using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    public interface IUserNicknameExtraStore<TUser, in TKey> : IUserStore<TUser, TKey> where TUser : class, IUser<TKey>
    {
        Task<TUser> FindByNicknameAsync(string nickname);
        Task<string> GetNicknameAsync(TUser user);
       
    }
}
