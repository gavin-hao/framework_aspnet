using Dongbo.Common;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity
{


    public class IdentityUser : IdentityUser<string>
    {
        public IdentityUser()
        {
            Id = IdGenerator.NextId().ToString();
        }

        public IdentityUser(string userName)
            : this()
        {
            UserName = userName;
        }

    }
    public class IdentityUser<TKey> : IUser<TKey> where TKey : IEquatable<TKey>
    {
        private readonly List<IdentityUserRole<TKey>> roles = new List<IdentityUserRole<TKey>>();
        private readonly List<IdentityUserLogin<TKey>> logins = new List<IdentityUserLogin<TKey>>();
        private readonly List<IdentityUserClaim<TKey>> claims = new List<IdentityUserClaim<TKey>>();
        public IdentityUser()
        {
            this.ConcurrencyStamp = Guid.NewGuid().ToString();

        }

        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }


        /// <summary>
        /// A random value that should change whenever a user is persisted to the store
        /// 
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; }

        /// <summary>
        /// Navigation property for users in the role
        /// 
        /// </summary>
        public virtual List<IdentityUserRole<TKey>> Roles { get { return roles; } }

        /// <summary>
        /// Navigation property for users claims
        /// 
        /// </summary>
        public virtual List<IdentityUserClaim<TKey>> Claims { get { return claims; } }

        /// <summary>
        /// Navigation property for users logins
        /// 
        /// </summary>
        public virtual List<IdentityUserLogin<TKey>> Logins { get { return logins; } }

        public virtual TKey Id { get; set; }

        public virtual string UserName { get; set; }

        public virtual string NormalizedUserName { get; set; }

        /// <summary>
        /// Email
        /// 
        /// </summary>
        public virtual string Email { get; set; }

        public virtual string NormalizedEmail { get; set; }

        /// <summary>
        /// True if the email is confirmed, default is false
        /// 
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// The salted/hashed form of the user password
        /// 
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// A random value that should change whenever a users credentials change (password changed, login removed)
        /// 
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        /// PhoneNumber for the user
        /// 
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// True if the phone number is confirmed, default is false
        /// 
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Is two factor enabled for the user
        /// 
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// 
        /// </summary>
        public virtual DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Is lockout enabled for this user
        /// 
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// Used to record failures for the purposes of lockout
        /// 
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// when to add 
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// user nickname
        /// </summary>
        public virtual string Nickname { get; set; }
        /// <summary>
        /// user data version default 1,if 0 old user system
        /// </summary>
        public virtual int Version
        {
            get;
            set;
        }
        /// <summary>
        /// indicate where the user came from (eg. "local","qq","wexin")
        /// </summary>
        public virtual string From
        {
            get;
            set;
        }
        /// <summary>
        /// Returns a friendly name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return UserName;
        }
        internal bool ClaimsLoaded { get; set; }
        internal bool RoleLaoded { get; set; }
        internal bool UserLoginLoaded { get; set; }
    }
}
