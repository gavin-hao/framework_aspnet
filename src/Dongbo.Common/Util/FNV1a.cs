using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dongbo.Common.Util
{
   public class FNV1a : HashAlgorithm
    {
        private const long Prime = 16777619L;
        private const long Offset = 2166136261L;
        protected long CurrentHashValue;
        public FNV1a()
        {
            this.HashSizeValue = 64;
            this.Initialize();
        }
        public override void Initialize()
        {
            this.CurrentHashValue = 2166136261L;
        }
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int num = ibStart + cbSize;
            for (int i = ibStart; i < num; i++)
            {
                this.CurrentHashValue = (this.CurrentHashValue ^ (uint)array[i]) * 16777619u;
            }
        }
        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(this.CurrentHashValue);
        }
    }
}

