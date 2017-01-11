using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public class DongboPrincipal : IPrincipal
    {
        private ClaimsIdentity identity;
        public DongboPrincipal()
        {
            this.identity = new ClaimsIdentity(string.Empty);
        }
        public DongboPrincipal(IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            this.identity = new ClaimsIdentity(identity);
        }
        public DongboPrincipal(IPrincipal principal)
            : this(principal.Identity)
        {
        }

        public static DongboPrincipal Current { get { return new DongboPrincipal(Thread.CurrentPrincipal); } }
        public static DongboPrincipal Anonymous { get { return new DongboPrincipal(new ClaimsIdentity()); } }
        public IIdentity Identity
        {
            get { return identity; }
        }

        public bool IsInRole(string role)
        {
            return this.identity.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == role);
        }

        /// <summary>Gets the time in UTC format when the identity expires.</summary>
        /// <value>The expire time in UTC format.</value>
        public DateTimeOffset ValidTo
        {
            get
            {

                var expireClaim = this.identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Expiration || x.Type == Constants.JwtClaimType.ExpirationTime);

                long unixTime;
                if (expireClaim != null && long.TryParse(expireClaim.Value, out unixTime))
                {
                    return unixTime.ToDateTimeOffset();
                }

                return DateTimeOffset.MinValue;

            }
        }

        /// <summary>Gets the roles.</summary>
        /// <value>The roles.</value>
        IEnumerable<string> Roles
        {
            get
            {
                return this.identity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            }
        }

        public override string ToString()
        {
            return this.identity.ToString();
        }
    }
}
