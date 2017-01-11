using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dongbo.Common.ExtensionMethod
{
    public static class StreamExtension
    {
        public static byte[] MD5ComputeHash(this Stream data)
        {
            var position = data.Position;
            data.Position = 0;
            var result = System.Security.Cryptography.MD5.Create().ComputeHash(data);
            data.Position = position;
            return result;
        }
        private const string DefaultBaseChars = "0123456789ABCDEF";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="partSize"></param>
        /// <returns>md5 hex string</returns>
        public static string ComputeContentMd5(this Stream input, long partSize)
        {
            using (var md5 = MD5.Create())
            {
                int readSize = (int)partSize;
                long pos = input.Position;
                byte[] buffer = new byte[readSize];
                readSize = input.Read(buffer, 0, readSize);

                var data = md5.ComputeHash(buffer, 0, readSize);
                var charset = DefaultBaseChars.ToCharArray();
                var sBuilder = new StringBuilder();
                foreach (var b in data)
                {
                    sBuilder.Append(charset[b >> 4]);
                    sBuilder.Append(charset[b & 0x0F]);
                }
                input.Seek(pos, SeekOrigin.Begin);
                return sBuilder.ToString();//Convert.ToBase64String(data);
            }
        }
    }
}
