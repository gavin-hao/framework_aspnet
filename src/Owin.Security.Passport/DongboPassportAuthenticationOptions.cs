using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Owin.Security.Passport
{
    public class DongboPassportAuthenticationOptions : AuthenticationOptions
    {
        public DongboPassportAuthenticationOptions() : this(DongboPassportAuthenticationDefaults.AuthenticationType) { }

        public DongboPassportAuthenticationOptions(string authenticationType)
            : base(authenticationType)
        {
            AuthenticationMode = AuthenticationMode.Passive;

            AuthorizationEndpoint = DongboPassportAuthenticationDefaults.AuthorizationEndpoint;
            SignOutEndpoint = DongboPassportAuthenticationDefaults.SignOutEndpoint;
            SignupEndpoint = DongboPassportAuthenticationDefaults.SignupEndpoint;
            SignupPath = new PathString("/signup");
            LoginPath = new PathString("/login");
            LogoutPath = new PathString("/logout");
            ReturnUrlParameter = "returnUrl";
        }

        public string AppId { get; set; }
        public PathString SignupPath { get; set; }
        public PathString LoginPath { get; set; }
        public PathString LogoutPath { get; set; }

        public string ReturnUrlParameter { get; set; }


        /// <summary>
        /// url to login 
        /// </summary>
        public string AuthorizationEndpoint { get; set; }

        public string SignOutEndpoint { get; set; }
        public string SignupEndpoint { get; set; }


    }
}
