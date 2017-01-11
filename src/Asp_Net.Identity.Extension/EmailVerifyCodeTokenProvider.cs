using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
namespace Dongbo.AspNet.Identity.Extension
{
    public class EmailVerifyCodeTokenProvider<TUser> : EmailTokenProvider<TUser> where TUser : class ,IUser<string>
    {
        public override async Task<string> GetUserModifierAsync(string purpose, UserManager<TUser, string> manager, TUser user)
        {
            var m = manager as ApplicationUserManager<TUser>;
            if (m != null)
            {
                var email = await m.GetEmailAsync(user);
                return "Email:" + purpose + ":" + email;
            }
            else
            {
                return await base.GetUserModifierAsync(purpose, manager, user);
            }


        }
    }
}
