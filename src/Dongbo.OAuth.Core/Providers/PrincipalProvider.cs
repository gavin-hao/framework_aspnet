using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Providers
{
    public class PrincipalProvider
    {
        private readonly ICryptoProvider cryptoProvider;
        
        private readonly Lazy<ClaimsPrincipal> current = new Lazy<ClaimsPrincipal>(() => ClaimsPrincipal.Current);

        public PrincipalProvider(ICryptoProvider cryptoProvider)
        {
            if (cryptoProvider == null)
            {
                throw new ArgumentNullException("cryptoProvider");
            }

            this.cryptoProvider = cryptoProvider;

        }
        public ClaimsPrincipal Anonymous
        {
            get
            {

                var principal = new ClaimsPrincipal();
                return principal;
            }
        }

        public ClaimsPrincipal Current
        {
            get
            {
                return current.Value;
            }
        }
        //private ClaimsIdentity AnonymosIdentity
        //{
        //    get { return new ClaimsIdentity(); }
        //}
        public DateTimeOffset PrincipalValidTo
        {
            get
            {
                var expireClaim = Current.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Expiration || x.Type == Constants.JwtClaimType.ExpirationTime);

                long unixTime;
                if (expireClaim != null && long.TryParse(expireClaim.Value, out unixTime))
                {
                    return unixTime.ToDateTimeOffset();
                }

                return DateTimeOffset.MinValue;
            }
        }

        public ClaimsPrincipal Create(string authenticationType, params Claim[] claims)
        {
            if (claims == null)
            {
                throw new ArgumentNullException("claims");
            }
            var identity = new ClaimsIdentity(claims, authenticationType);
            return new ClaimsPrincipal(identity);
        }

        public string Encrypt(ClaimsPrincipal principal, string key)
        {
            var s = JsonConvert.SerializeObject(principal);

            return this.cryptoProvider.Encrypt(s, key);
        }

        public ClaimsPrincipal Decrypt(string ticket, string key)
        {
            var s = this.cryptoProvider.Decrypt(ticket, key);

            return JsonConvert.DeserializeObject<ClaimsPrincipal>(s);
        }
    }
}
