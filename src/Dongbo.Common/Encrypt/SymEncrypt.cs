using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Dongbo.Common.Encrypt
{
    /// <summary>
    /// 对称加密算法枚举
    /// </summary>
    public enum SymEncryptionAlgorithm
    {
        DES = 0, RC2, Rijndael, TripleDes
    }

    /// <summary>
    /// symmetric encryption 私钥加密（对称加密）  
    /// </summary>
    public interface ISymEncrypt
    {
        /// <summary>
        /// 对字符串进行加密
        /// </summary>
        /// <param name="source">要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        string Encrypt(string source);

        /// <summary>
        /// 对字符串进行解密
        /// </summary>
        /// <param name="source">要解密的字符串</param>
        /// <returns>返回解密后的字符串</returns>
        string Decrypt(string source);
    }

    /// <summary>
    /// 对称加密的实现
    /// </summary>
    public class SymEncrypt : ISymEncrypt
    {
        private string _key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";
        private string _iv = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";

        private SymEncryptionAlgorithm _algorithm = SymEncryptionAlgorithm.Rijndael;
        private SymmetricAlgorithm _cryptAlgorithm;
        public static SymEncrypt GetDefault()
        {
            return new SymEncrypt();
        }
        public SymEncrypt()
        {
            _cryptAlgorithm = GetSymAlgorithm();
            _cryptAlgorithm.Key = GetLegalKey();
            _cryptAlgorithm.IV = GetLegalIV();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">加密用的Key</param>
        /// <param name="iv">加密用的IV，一段字符串，要和相应的Key匹配，加解密必须用同一段Key和IV</param>
        /// <param name="algorithm">加密算法，默认是Rijndael</param>
        public SymEncrypt(string key, string iv, SymEncryptionAlgorithm algorithm)
        {
            if (!string.IsNullOrEmpty(key))
                _key = key;

            if (!string.IsNullOrEmpty(iv))
                _iv = iv;

            _algorithm = algorithm;
            _cryptAlgorithm = GetSymAlgorithm();
            _cryptAlgorithm.Key = GetLegalKey();
            _cryptAlgorithm.IV = GetLegalIV();
        }

        private SymmetricAlgorithm GetSymAlgorithm()
        {
            switch (_algorithm)
            {
                case SymEncryptionAlgorithm.Rijndael:
                    return new RijndaelManaged();
                case SymEncryptionAlgorithm.DES:
                    return new DESCryptoServiceProvider();
                case SymEncryptionAlgorithm.RC2:
                    return new RC2CryptoServiceProvider();
                case SymEncryptionAlgorithm.TripleDes:
                    return new TripleDESCryptoServiceProvider();
                default:
                    return new RijndaelManaged();
            }
        }

        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            try
            {
                _cryptAlgorithm.GenerateKey();
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace);
            }
            byte[] tempKey = _cryptAlgorithm.Key;
            int keyLength = tempKey.Length;
            string key = _key;
            if (_key.Length > keyLength)
                key = key.Substring(0, keyLength);
            else
                key = key.PadRight(keyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(key);

        }
        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string iv = _iv;
            _cryptAlgorithm.GenerateIV();
            byte[] bytTemp = _cryptAlgorithm.IV;
            int ivLength = bytTemp.Length;
            if (iv.Length > ivLength)
                iv = iv.Substring(0, ivLength);
            else
                iv = iv.PadRight(ivLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(iv);
        }

        private ICryptoTransform Encryptor
        {
            get
            {
                return _cryptAlgorithm.CreateEncryptor();
            }
        }

        private ICryptoTransform Decryptor
        {
            get
            {
                return _cryptAlgorithm.CreateDecryptor();
            }
        }


        #region ISymEncrypt Members

        /// <summary>
        /// 对字符串进行加密
        /// </summary>
        /// <param name="source">要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public string Encrypt(string source)
        {
            byte[] byteIn = UTF8Encoding.UTF8.GetBytes(source);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cs = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cs.Write(byteIn, 0, byteIn.Length);
            cs.FlushFinalBlock();
            memoryStream.Close();
            byte[] bytOut = memoryStream.ToArray();
            memoryStream.Close();
            return Convert.ToBase64String(bytOut);

        }

        /// <summary>
        /// 对字符串进行解密
        /// </summary>
        /// <param name="source">要解密的字符串</param>
        /// <returns>返回解密后的字符串</returns>
        public string Decrypt(string source)
        {
            byte[] byteIn = Convert.FromBase64String(source);
            MemoryStream ms = new MemoryStream(byteIn, 0, byteIn.Length);
            CryptoStream cs = new CryptoStream(ms, Decryptor, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            string result = sr.ReadToEnd();
            cs.Close();
            sr.Close();
            return result;
        }

        #endregion
    }
}
