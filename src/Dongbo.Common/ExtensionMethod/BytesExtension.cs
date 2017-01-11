using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Common.ExtensionMethod
{
    public static class BytesExtension
    {
        public static byte[] MD5ComputeHash(this byte[] data)
        {
            return System.Security.Cryptography.MD5.Create().ComputeHash(data);
        }
        public static string ToHexString(this byte[] data)
        {
            return data.Select(m => m.ToString("X2")).Join("");
        }
    }
}
