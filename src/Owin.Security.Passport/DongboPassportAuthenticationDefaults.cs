using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Owin.Security.Passport
{
    public static class DongboPassportAuthenticationDefaults
    {
        /// <summary>
        ///  The default value used for <see cref="> DongboPassportAuthenticationOptions.AuthenticationType"/>
        /// </summary>
        public const string AuthenticationType = "AxeSlide";

        public const string SignupEndpoint = "http://accounts.axeslide.com/register";
        public const string AuthorizationEndpoint = "http://accounts.axeslide.com/login";
        public const string SignOutEndpoint = "http://accounts.axeslide.com/logout";
    }
}
