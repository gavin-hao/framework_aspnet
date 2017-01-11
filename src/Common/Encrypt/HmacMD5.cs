using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Common.Encrypt
{
    public class HmacMD5
    {
        public static string HMAC(string text, string key)
        {
            string ipad = String.Empty;
            string opad = String.Empty;
            string iResult = String.Empty;
            string oResult = String.Empty;

            for (int i = 0; i < 64; i++)
            {
                ipad += "6";
                opad += "\\";
            }

            int KLen = key.Length;

            for (int i = 0; i < 64; i++)
            {
                if (i < KLen)
                    iResult += Convert.ToChar(ipad[i] ^ key[i]);
                else
                    iResult += Convert.ToChar(ipad[i]);
            }

            iResult += text;
            iResult = fun_MD5(iResult);

            byte[] Test = Hexstr2Array(iResult);
            iResult = String.Empty;
            char[] b = System.Text.Encoding.GetEncoding(1252).GetChars(Test);

            for (int i = 0; i < b.Length; i++)
            {
                iResult += b[i];
            }

            for (int i = 0; i < 64; i++)
            {
                if (i < KLen)
                    oResult += Convert.ToChar(opad[i] ^ key[i]);
                else
                    oResult += Convert.ToChar(opad[i]);
            }

            oResult += iResult;
            return fun_MD5(oResult);

        }
        private static string fun_MD5(string str)
        {
            //1252
            byte[] byteArray = System.Text.Encoding.GetEncoding(1252).GetBytes(str);
            byteArray = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(byteArray);
            string ret = "";
            for (int i = 0; i < byteArray.Length; i++)
                ret += byteArray[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        private static Byte[] Hexstr2Array(string HexStr)
        {
            string HEX = "0123456789ABCDEF";
            string str = HexStr.ToUpper();
            int len = str.Length;
            byte[] retByte = new byte[len / 2];
            for (int i = 0; i < len / 2; i++)
            {
                int numHigh = HEX.IndexOf(str[i * 2]);
                int numLow = HEX.IndexOf(str[i * 2 + 1]);
                retByte[i] = Convert.ToByte(numHigh * 16 + numLow);
            }
            return retByte;
        }

    }
}
