using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    /// <summary>
    /// Configured how cookies are managed by IdentityServer.
    /// </summary>
    //public class CookieOptions
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="CookieOptions"/> class.
    //    /// </summary>
    //    public CookieOptions()
    //    {
    //        ExpireTimeSpan = DongboAuthenticationDefaults.DefaultCookieTimeSpan;
    //        SlidingExpiration = false;
    //        AllowRememberMe = true;
    //        RememberMeDuration = DongboAuthenticationDefaults.DefaultRememberMeDuration;
    //        SecureMode = CookieSecureOption.SameAsRequest;
    //        Prefix = ".AxeSlide.";
    //    }

    //    /// <summary>
    //    /// Allows setting a prefix on cookies to avoid potential conflicts with other cookies with the same names.
    //    /// </summary>
    //    /// <value>
    //    /// The prefix.
    //    /// </value>
    //    public string Prefix { get; set; }

    //    /// <summary>
    //    /// The expiration duration of the authentication cookie. Defaults to 10 hours.
    //    /// </summary>
    //    /// <value>
    //    /// The expire time span.
    //    /// </value>
    //    public TimeSpan ExpireTimeSpan { get; set; }

    //    /// <summary>
    //    /// Gets or sets a value indicating whether the authentication cookie is marked as persistent. Defaults to <c>false</c>.
    //    /// </summary>
    //    /// <value>
    //    /// <c>true</c> if persistent; otherwise, <c>false</c>.
    //    /// </value>
    //    public bool IsPersistent { get; set; }

    //    /// <summary>
    //    /// Gets or sets a value indicating if the authentication cookie is sliding, which means it auto renews as the user is active. Defaults to <c>false</c>.
    //    /// </summary>
    //    /// <value>
    //    ///   <c>true</c> if sliding; otherwise, <c>false</c>.
    //    /// </value>
    //    public bool SlidingExpiration { get; set; }

    //    /// <summary>
    //    /// Gets or sets the cookie path.
    //    /// </summary>
    //    /// <value>
    //    /// The cookie path.
    //    /// </value>
    //    public string Path { get; set; }

    //    /// <summary>
    //    /// Gets or sets a value indicating whether the "remember me" option is presented to users on the login page. 
    //    /// If selected this option will issue a persistent authentication cookie. Defaults to true.
    //    /// </summary>
    //    /// <value>
    //    ///   <c>true</c> if allowed; otherwise, <c>false</c>.
    //    /// </value>
    //    public bool AllowRememberMe { get; set; }

    //    /// <summary>
    //    /// Gets or sets the duration of the persistent cookie issued by the "remember me" option on the login page.
    //    /// Defaults to 30 days.
    //    /// </summary>
    //    /// <value>
    //    /// The duration of the "remember me" persistent cookie.
    //    /// </value>
    //    public TimeSpan RememberMeDuration { get; set; }

    //    /// <summary>
    //    /// Gets or sets the mode for issuing the secure flag on the cookies issued. Defaults to SameAsRequest.
    //    /// </summary>
    //    /// <value>
    //    /// The secure.
    //    /// </value>
    //    public CookieSecureOption SecureMode { get; set; }

    //    /// <summary>
    //    /// An optional container in which to store the identity across requests. When used, only a session identifier is sent
    //    /// to the client. This can be used to mitigate potential problems with very large identities.
    //    /// </summary>
    //    public IAuthenticationSessionStore SessionStoreProvider { get; set; }

    //    public CookieAuthenticationProvider CookieAuthenticationProvider { get; set; }


    //    public string LoginPath { get; set; }

    //    public string LogoutPath { get; set; }
    //}
}
