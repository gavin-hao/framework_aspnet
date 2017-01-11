using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Providers
{
    public class SimpleCryptoProvider:ICryptoProvider
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        private readonly RandomNumberGenerator rng;

        public string CreateHash(out string text, int length)
        {
            throw new NotImplementedException();
        }

        public string CreateHash(int length)
        {
            throw new NotImplementedException();
        }

        public string CreateHash(string text, bool useSalt = true)
        {
            throw new NotImplementedException();
        }

        public bool ValidateHash(string text, string correctHash)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string text, string key)
        {
            throw new NotImplementedException();
        }

        public string Decrypt(string ticket, string key)
        {
            throw new NotImplementedException();
        }
    }
}
