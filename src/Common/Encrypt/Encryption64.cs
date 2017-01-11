using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Dongbo.Common.Encrypt
{
    public class Encryption64
    {
        public static string encryptQueryString(string strQueryString)
        {
            return Encryption64.Encrypt(strQueryString, "!2#$56&*9");
        }
        public static string decryptQueryString(string strQueryString)
        {
            return Encryption64.Decrypt(strQueryString, "!2#$56&*9");
        }
        public static string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            stringToDecrypt = stringToDecrypt.Replace("NBAM", "+");//.Replace("MABN"," ").Replace("MMMV","/").Replace("VMMM","=");
            byte[] key = { };
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray = new byte[stringToDecrypt.Length];

            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static string Encrypt(string stringToEncrypt, string sEncryptionKey)
        {

            byte[] key = { };
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray; //Convert.ToByte(stringToEncrypt.Length)

            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] strPre = ms.ToArray();
                //string strValue=Convert.ToBase64String(ms.ToArray()).Replace("+", "%2B");
                string strValue = Convert.ToBase64String(ms.ToArray()).Replace("+", "NBAM");//.Replace(" ","MABN").Replace("/","MMMV").Replace("=","VMMM");
                return strValue;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
