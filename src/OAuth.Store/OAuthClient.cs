using Dongbo.OAuth.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Store
{
    public class OAuthClient : IClient
    {
        public OAuthClient()
        {
        }

        /// <summary>Initializes a new instance of the Sentinel.OAuth.Core.Models.OAuth.Client class.</summary>
        /// <param name="client">The client.</param>
        public OAuthClient(IClient client)
        {
            this.Id = client.Id;
            this.ClientId = client.ClientId;
            this.ClientSecret = client.ClientSecret;
            this.RedirectUri = client.RedirectUri;
            this.Name = client.Name;
            this.LastUsed = client.LastUsed;
            this.Enabled = client.Enabled;
            this.CreateTime = DateTime.Now;
            this.Description = client.Description;
            this.OwnerId = client.OwnerId;
            this.HomeUrl = client.HomeUrl;
        }
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string HomeUrl { get; set; }
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the redirect uri.
        /// </summary>
        /// <value>The redirect uri.</value>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last used date.
        /// </summary>
        /// <value>The last used date.</value>
        public DateTimeOffset LastUsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this Client is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        public string Description { get; set; }

        /// <summary>Gets the identifier.</summary>
        /// <returns>The identifier.</returns>
        public virtual string GetIdentifier()
        {
            return string.Format("{0}|{1}", this.ClientId, this.RedirectUri);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual bool Equals(IClient other)
        {
            if (this.ClientId == other.ClientId && this.RedirectUri == other.RedirectUri)
            {
                return true;
            }

            return false;
        }



    }
}
