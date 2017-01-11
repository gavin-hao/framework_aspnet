using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Common.ExtensionMethod
{
    public static class Base64StringExtensions
    {
        public static string ToBase64String(this long fromNumber)
        {
            byte[] bArry = BitConverter.GetBytes(fromNumber);
            var b64 = Convert.ToBase64String(bArry);
            return b64;
        }
        public static long ConvertToInt64(this string base64string)
        {
            var bArray = Convert.FromBase64String(base64string);
            var int64 = BitConverter.ToInt64(bArray, 0);
            return int64;
        }
        public static string Base64ToNormalString(this string base64String)
        {
            return base64String.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
        public static string ToBase64String(this string normalString)
        {
            return Pad(normalString.Replace('-', '+').Replace('_', '/'));
        }
        private static string Pad(string text)
        {
            var padding = 3 - ((text.Length + 3) % 4);
            if (padding == 0)
            {
                return text;
            }
            return text + new string('=', padding);
        }
        public static string Base64ToString( string base64String)
        {
            if(string.IsNullOrWhiteSpace(base64String))
            {
                return null;
            }
            char[] charBuffer = base64String.ToCharArray();
            byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
            string returnstr2 = Encoding.Default.GetString(bytes);
            return returnstr2;
        } 

    }
}
