using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Models
{
    public class AccessToken : IAccessToken
    {
        public AccessToken(IAccessToken accessToken)
        {
            this.ClientId = accessToken.ClientId;
            this.RedirectUri = accessToken.RedirectUri;
            this.Subject = accessToken.Subject;
            this.Token = accessToken.Token;
            this.Ticket = accessToken.Ticket;
            //this.ValidTo = accessToken.ValidTo;
            this.Scope = accessToken.Scope;
            this.ExpiresIn = accessToken.ExpiresIn;
            this.IssuedAt = accessToken.IssuedAt;
        }
        public AccessToken()
        {

        }
        public string ClientId { get; set; }
        public string Ticket { get; set; }

        public string RedirectUri { get; set; }
        public string Token { get; set; }
        public string Subject { get; set; }

        public IEnumerable<string> Scope { get; set; }

        public DateTimeOffset ExpiresIn { get; set; }


        public DateTime IssuedAt { get; set; }

        public DateTimeOffset ValidTo
        {
            get { return this.ExpiresIn; }
            set { this.ExpiresIn = value; }
        }

        public virtual string GetIdentifier()
        {
            return this.Token;
        }



        public virtual bool IsValid()
        {
            if (this.ClientId == null
                || (this.RedirectUri == null && this.Scope == null)
                || this.Subject == null
                || this.Token == null
                || this.Ticket == null
                || this.ValidTo == DateTimeOffset.MinValue)
            {
                return false;
            }

            return true;
        }

        public bool Equals(IAccessToken other)
        {
            if (this.GetIdentifier() != other.GetIdentifier())
            {
                return true;
            }

            return false;
        }



    }
}
