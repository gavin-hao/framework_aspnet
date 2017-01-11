using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Owin.Security.AesDataProtectionProvider
{
    /// <summary>
    /// Provider AES DataProtectorProvider
    /// </summary>
    public class AesDataProtectionProvider : IDataProtectionProvider
    {
        private string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="AesDataProtectionProvider" /> class.
        /// </summary>
        /// <param name="key">The key used to encrypto.</param>
        public AesDataProtectionProvider(string appKey = null)
        {
            _key = appKey;
        }

        private string SeedHash
        {

            get
            {
                if (string.IsNullOrWhiteSpace(_key))
                    _key = HashString(Environment.MachineName);

                return _key;
            }
        }
        private string HashString(string value)
        {
            return HexStringFromBytes(new SHA512CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(value))).ToUpper();
        }

        /// <summary>
        /// Returns a new instance of IDataProtection for the provider.
        /// </summary>
        /// <param name="purposes">Additional entropy used to ensure protected data may only be unprotected for the correct purposes.</param>
        /// <returns>
        /// An instance of a data protection service
        /// </returns>
        public IDataProtector Create(params string[] purposes)
        {
            return new AesDataProtector(SeedHash + ":" + string.Join(",", purposes));
        }

        /// <summary>
        /// Convert an array of bytes to a string of hex digits
        /// </summary>
        /// <param name="bytes">array of bytes</param>
        /// <returns>String of hex digits</returns>
        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();

            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
