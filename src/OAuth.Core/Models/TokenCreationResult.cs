using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Models
{
    public class TokenCreationResult<T> where T : IToken
    {
        /// <summary>Initializes a new instance of the TokenCreationResult class.</summary>
        /// <param name="token">The token.</param>
        /// <param name="entity">The entity.</param>
        public TokenCreationResult(string token, T entity)
        {
            this.Token = token;
            this.Entity = entity;
        }

        /// <summary>Gets the token.</summary>
        /// <value>The token.</value>
        public string Token { get; private set; }

        /// <summary>Gets the entity.</summary>
        /// <value>The entity.</value>
        public T Entity { get; private set; }
    }
}
