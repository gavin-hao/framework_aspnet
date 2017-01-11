
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
namespace Dongbo.Common.Encrypt
{
    public class Cryption
    {

        private static string strKey = "8635yn2G2s|l6t$fj29s92l4%5o|Ao432";
        /// <summary>
        /// 密钥
        /// </summary>
        public Cryption()
        {

        }
        /// <summary>
        /// 指定用于加密的块密码模式。34432d
        /// </summary>
        /// <param name="str">需要加密前的文本</param>
        /// <returns>加密后的文本</returns>
        public static string EncryptTripleDES(string str)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            DES.Key = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(strKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
        /// <summary>
        /// 指定用于解密的块密码模式。
        /// </summary>
        /// <param name="str">需要解密前的文本</param>
        /// <returns>解密后的文本</returns>
        public static string DecryptTripleDES(string base64Text)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            DES.Key = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(strKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            byte[] Buffer = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strString">原始文本</param>
        /// <returns>MD5加密串</returns>
        public static string EncryptStrMD5(string strString)
        {
            if (!string.IsNullOrEmpty(strString))
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strString, "MD5");
            }
            else
            {
                return strString;
            }
        }
    }
}
