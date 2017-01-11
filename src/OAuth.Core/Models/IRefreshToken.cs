using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public interface IRefreshToken : IToken, IEquatable<IRefreshToken>
    {
        /// <summary>Gets or sets the token.</summary>
        /// <value>The token.</value>
        string Token { get; set; }
    }
}
