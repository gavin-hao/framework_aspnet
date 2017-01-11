using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Models
{
    public class TokenValidationResult<T> where T : IToken
    {

        public TokenValidationResult(T entity)
        {
            this.Entity = entity;
        }
        public bool IsValidated { get; private set; }
        /// <summary>
        /// True if application code has called any of the SetError methods on this context.
        /// </summary>
        public bool HasError { get; private set; }

        /// <summary>
        /// The error argument provided when SetError was called on this context. This is eventually
        /// returned to the client app as the OAuth "error" parameter.
        /// </summary>
        public string Error { get; private set; }
        /// <summary>Gets the entity.</summary>
        /// <value>The entity.</value>
        public T Entity { get; private set; }

        /// <summary>Determines if the result is valid.</summary>
        public virtual bool Validated()
        {
            IsValidated = true;
            HasError = false;
            return true;
        }
        /// <summary>
        /// Marks this context as not validated by the application. IsValidated and HasError become false as a result of calling.
        /// </summary>
        public virtual void Rejected()
        {
            IsValidated = false;
            HasError = false;
        }
    }
}
