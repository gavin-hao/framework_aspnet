﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Models
{
    public class RefreshToken : IRefreshToken
    {
        public RefreshToken()
        {
        }


        public RefreshToken(IRefreshToken refreshToken)
        {
            this.ClientId = refreshToken.ClientId;
            this.RedirectUri = refreshToken.RedirectUri;
            this.Subject = refreshToken.Subject;
            this.Token = refreshToken.Token;
            this.ExpiresIn = refreshToken.ExpiresIn;
            this.IssuedAt = refreshToken.IssuedAt;
            this.ValidTo = refreshToken.ValidTo;
            this.Scope = refreshToken.Scope;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>The redirect URI.</value>
        public string RedirectUri { get; set; }

        /// <summary>Gets or sets the scope.</summary>
        /// <value>The scope.</value>
        public IEnumerable<string> Scope { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; set; }
        public DateTimeOffset ExpiresIn { get; set; }


        public DateTime IssuedAt { get; set; }
        /// <summary>
        /// Gets or sets the expiration time.
        /// </summary>
        /// <value>The expiration time.</value>
        public DateTimeOffset ValidTo
        {
            get
            {
                return this.ExpiresIn;
            }
            set { this.ExpiresIn = value; }
        }

        /// <summary>Gets the identifier.</summary>
        /// <returns>The identifier.</returns>
        public virtual string GetIdentifier()
        {
            return this.Token;
        }

        /// <summary>Check if this object is valid.</summary>
        /// <returns><c>true</c> if valid, <c>false</c> if not.</returns>
        public virtual bool IsValid()
        {
            if (this.ClientId == null
                || (this.RedirectUri == null && this.Scope == null)
                || this.Subject == null
                || this.Token == null
                || this.ValidTo == DateTimeOffset.MinValue)
            {
                return false;
            }

            return true;
        }

        /// <summary>Tests if this IRefreshToken is considered equal to another.</summary>
        /// <param name="other">The i refresh token to compare to this object.</param>
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        public virtual bool Equals(IRefreshToken other)
        {
            if (this.GetIdentifier() == other.GetIdentifier())
            {
                return true;
            }

            return false;
        }
    }
}

