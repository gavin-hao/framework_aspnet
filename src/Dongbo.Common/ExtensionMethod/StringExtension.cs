using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Common.ExtensionMethod
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        private static readonly string Seq62Random = "s9LFkgy5RovixI1aOf8UhdY3r4DMplQZJXPqebE0WSjBn7wVzmN2Gc6THCAKut";
        private static readonly string Seq62 ="0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// 10进制转换为62进制
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToBase62(this long id,bool randomSeed=false)
        {
            var Seq = randomSeed ? Seq62Random : Seq62;

            if (id < 62)
            {
                return Seq[(int)id].ToString();
            }
            int y = (int)(id % 62);
            long x = (long)(id / 62);
            return ToBase62(x) + Seq[y];
        }

        /// <summary>
        /// 将62进制转为10进制
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="randomSeed">是否打乱62进制序列</param>
        /// <returns></returns>
        public static long FromBase62(this string Num, bool randomSeed = false)
        {
            var Seq = randomSeed ? Seq62Random : Seq62;
            long v = 0;
            int Len = Num.Length;
            for (int i = Len - 1; i >= 0; i--)
            {
                int t = Seq.IndexOf(Num[i]);
                double s = (Len - i) - 1;
                long m = (long)(Math.Pow(62, s) * t);
                v += m;
            }
            return v;
        }
        public static string Mixup(this string Key)
        {
           
            int s = 0;
            foreach (char c in Key)
            {
                s += (int)c;
            }
            int Len = Key.Length;
            int x = (s % Len);
            char[] arr = Key.ToCharArray();
            char[] newarr = new char[arr.Length];
            Array.Copy(arr, x, newarr, 0, Len - x);
            Array.Copy(arr, 0, newarr, Len - x, x);
            string NewKey = "";
            foreach (char c in newarr)
            {
                NewKey += c;
            }
            return NewKey;
        }
        public static string UnMixup(this string Key)
        {
            int s = 0;
            foreach (char c in Key)
            {
                s += (int)c;
            }
            int Len = Key.Length;
            int x = (s % Len);
            x = Len - x;
            char[] arr = Key.ToCharArray();
            char[] newarr = new char[arr.Length];
            Array.Copy(arr, x, newarr, 0, Len - x);
            Array.Copy(arr, 0, newarr, Len - x, x);
            string NewKey = "";
            foreach (char c in newarr)
            {
                NewKey += c;
            }
            return NewKey;
        }
    }

     
}
