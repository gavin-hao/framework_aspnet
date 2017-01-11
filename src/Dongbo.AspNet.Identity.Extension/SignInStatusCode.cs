using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{

    public class SignInResult<TUser> where TUser : class, IUser<string>
    {
        public TUser User { get; set; }
        public SignInStatusCode StatusCode { get; set; }
    }
    public enum SignInStatusCode
    {
        // Summary:
        //     Sign in was successful
        Success = 0,
        //
        // Summary:
        //     User is locked out
        LockedOut = 1,
        //
        // Summary:
        //     Sign in requires addition verification (i.e. two factor)
        RequiresVerification = 2,
        //
        // Summary:
        //     Sign in failed
        Failure = 3,
        /// <summary>
        /// email not confirmed
        /// </summary>
        InvalidEmail = 4,
    }
}
