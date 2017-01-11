using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public interface IUserManager
    {
        /// <summary>
        /// Authenticates the user using username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The user principal.</returns>
        Task<ClaimsIdentity> AuthenticateUserWithPasswordAsync(string username, string password);

        /// <summary>
        /// Authenticates the user using username only.
        /// This method is used to get new user claims after a refresh token has been used. You can therefore assume that the user is already logged in.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user principal.</returns>
        Task<ClaimsIdentity> AuthenticateUserAsync(string username);
    }
}
