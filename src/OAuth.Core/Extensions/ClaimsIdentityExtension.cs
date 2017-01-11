using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Extensions
{
    internal static class ClaimsIdentityExtension
    {
        public static void RemoveClaim(this ClaimsIdentity identity, Predicate<Claim> condition)
        {
            var claims = identity.FindAll(condition);
            if (claims != null && claims.Count() > 0)
            {
                foreach (var c in claims)
                {
                    identity.RemoveClaim(c);
                }
            }
        }

        public static void AddClaim(this ClaimsIdentity identity,string name ,string value)
        {
            identity.AddClaim(new Claim(name, value));
           
        }
    }
}
