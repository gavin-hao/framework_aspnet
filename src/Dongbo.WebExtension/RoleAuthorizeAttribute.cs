using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dongbo.WebExtension
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        internal bool PerformAuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return this.AuthorizeCore(httpContext);
        }
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            //if (httpContext == null)
            //{
            //    throw new ArgumentNullException("httpContext");
            //}

            //IPrincipal user = httpContext.User;
            //if (!user.Identity.IsAuthenticated)
            //{
            //    return false;
            //}

            //if (_usersSplit.Length > 0 && !_usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            //{
            //    return false;
            //}

            //if (_rolesSplit.Length > 0 && !_rolesSplit.Any(user.IsInRole))
            //{
            //    return false;
            //}

            //return true;
            return base.AuthorizeCore(httpContext);
        }
    }
}
