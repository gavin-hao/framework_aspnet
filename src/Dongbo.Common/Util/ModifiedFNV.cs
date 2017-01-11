using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Common.Util
{
    public class ModifiedFNV : FNV1a
    {
        protected override byte[] HashFinal()
        {
            this.CurrentHashValue += this.CurrentHashValue << 13;
            this.CurrentHashValue ^= this.CurrentHashValue >> 7;
            this.CurrentHashValue += this.CurrentHashValue << 3;
            this.CurrentHashValue ^= this.CurrentHashValue >> 17;
            this.CurrentHashValue += this.CurrentHashValue << 5;
            return base.HashFinal();
        }
    }
}
