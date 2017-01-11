using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.AspNet.Identity
{
    public class IdentityUserRole<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// UserId for the user that is in the role
        /// 
        /// </summary>
        public virtual TKey UserId { get; set; }

        /// <summary>
        /// RoleId for the role
        /// 
        /// </summary>
        public virtual TKey RoleId { get; set; }


        public virtual string RoleName { get; set; }
    }
}
