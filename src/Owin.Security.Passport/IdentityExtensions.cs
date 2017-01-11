using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Owin.Security.AxeSlide
{
    //public static class IdentityExtensions
    //{
    //    public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
    //    {
    //        if (identity == null)
    //        {
    //            throw new ArgumentNullException("identity");
    //        }
    //        var claim = identity.FindFirst(claimType);
    //        return claim != null ? claim.Value : null;
    //    }

    //    public static string GetUserId(this IIdentity identity)
    //    {
    //        if (identity == null)
    //        {
    //            throw new ArgumentNullException("identity");
    //        }
    //        var ci = identity as ClaimsIdentity;
    //        if (ci != null)
    //        {
    //            return ci.FindFirstValue(ClaimTypes.NameIdentifier);
    //        }
    //        return null;
    //    }
    //    public static string GetUserName(this IIdentity identity)
    //    {
    //        if (identity == null)
    //        {
    //            throw new ArgumentNullException("identity");
    //        }
    //        var ci = identity as ClaimsIdentity;
    //        if (ci != null)
    //        {
    //            return ci.FindFirstValue(ClaimTypes.Name);
    //        }
    //        return null;
    //    }
    //    public static string GetUserEmail(this IIdentity identity)
    //    {
    //        if (identity == null)
    //        {
    //            throw new ArgumentNullException("identity");
    //        }
    //        var ci = identity as ClaimsIdentity;
    //        if (ci != null)
    //        {
    //            return ci.FindFirstValue(ClaimTypes.Email);
    //        }
    //        return null;
    //    }

    //    public static string GetUserPhoneNumber(this IIdentity identity)
    //    {
    //        if (identity == null)
    //        {
    //            throw new ArgumentNullException("identity");
    //        }
    //        var ci = identity as ClaimsIdentity;
    //        if (ci != null)
    //        {
    //            return ci.FindFirstValue(ClaimTypes.MobilePhone);
    //        }
    //        return null;
    //    }


    //}
}
