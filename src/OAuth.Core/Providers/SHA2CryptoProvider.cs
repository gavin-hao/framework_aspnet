using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Providers
{
    /// <summary>
    /// A <c>SHA-2</c> crypto provider for creating and validating hashes.
    /// </summary>
    public class SHA2CryptoProvider : ICryptoProvider
    {
        private int saltByteSize = 64;

        private readonly RandomNumberGenerator rng;

        public SHA2CryptoProvider()
        {
            this.rng = new RNGCryptoServiceProvider();
        }

        public string CreateHash(out string text, int length)
        {
            text = this.GenerateText(length);

            return this.CreateHash(text);
        }

        public string CreateHash(int length)
        {
            var text = this.GenerateText(length);

            return this.CreateHash(text, false);
        }

        public string CreateHash(string text, bool useSalt = true)
        {
            byte[] hash;

            if (useSalt)
            {
                // Generate a random salt
                var salt = this.GenerateSalt();

                // Hash the password and encode the parameters
                hash = this.Compute(Encoding.UTF8.GetBytes(text), salt);
            }
            else
            {
                hash = this.Compute(Encoding.UTF8.GetBytes(text));
            }

            var result = Convert.ToBase64String(hash);


            return result;
        }

        public bool ValidateHash(string text, string correctHash)
        {

            byte[] saltedHash;

            try
            {
                saltedHash = Convert.FromBase64String(correctHash);
            }
            catch (FormatException ex)
            {
                return false;
            }

            // Get salt from the end of the hash
            var offset = saltedHash.Length - this.saltByteSize;
            var saltBytes = new byte[this.saltByteSize];
            Buffer.BlockCopy(saltedHash, offset, saltBytes, 0, this.saltByteSize);

            // Compute new hash from the supplied text and the extracted salt
            var newHash = this.Compute(Encoding.UTF8.GetBytes(text), saltBytes);

            // If the hashes match, the text is valid
            var result = saltedHash.SequenceEqual(newHash);

            return result;
        }

        public string Encrypt(string text, string key)
        {
            // Create random key generator
            var pdb = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(key));

            // Encrypt the principal
            byte[] encrypted;

            using (var rijAlg = new RijndaelManaged() { Key = pdb.GetBytes(32), IV = pdb.GetBytes(16) })
            {
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string ticket, string key)
        {

            string decryptedText;

            // Create random key generator
            var pdb = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(key));

            using (var rijAlg = new RijndaelManaged() { Key = pdb.GetBytes(32), IV = pdb.GetBytes(16) })
            {
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(ticket)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            decryptedText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedText;
        }

        /// <summary>
        /// Generates a random text.
        /// </summary>
        /// <param name="length">The text length.</param>
        /// <returns>The random text.</returns>
        private string GenerateText(int length)
        {
            const string AllowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";

            var max = length / 8;
            var text = new byte[max];

            for (var i = 0; i < max; i++)
            {
                var index = this.GetRandomIndex(0, AllowedChars.Length);

                text[i] = Convert.ToByte(AllowedChars[index]);
            }

            return Encoding.UTF8.GetString(text);
        }

        /// <summary>
        /// Generates a random salt.
        /// </summary>
        /// <returns>The random salt.</returns>
        private byte[] GenerateSalt()
        {
            var csprng = new RNGCryptoServiceProvider();
            var salt = new byte[this.saltByteSize];

            csprng.GetBytes(salt);

            return salt;
        }

        /// <summary>Computes the hash of a text.</summary>
        /// <param name="text">The text to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>A hash of the text.</returns>
        private byte[] Compute(byte[] text)
        {
            using (var sha = this.GetCryptoServiceProvider())
            {
                return sha.ComputeHash(text);
            }
        }

        /// <summary>Computes the hash of a text.</summary>
        /// <param name="text">The text to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>A hash of the text.</returns>
        private byte[] Compute(byte[] text, byte[] salt)
        {
            using (var sha = this.GetCryptoServiceProvider())
            {
                // Prepend salt to text
                var saltedText = new byte[salt.Length + text.Length];
                Buffer.BlockCopy(salt, 0, saltedText, 0, salt.Length);
                Buffer.BlockCopy(text, 0, saltedText, salt.Length, text.Length);

                // Create hash
                var hash = sha.ComputeHash(saltedText);

                // Append salt to hash
                var saltedHash = new byte[hash.Length + salt.Length];
                Buffer.BlockCopy(hash, 0, saltedHash, 0, hash.Length);
                Buffer.BlockCopy(salt, 0, saltedHash, hash.Length, salt.Length);

                return saltedHash;
            }
        }

        /// <summary>
        /// Gets a random index.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The index.</returns>
        private int GetRandomIndex(int minValue, int maxValue)
        {
            const long Max = 1 + (long)uint.MaxValue;

            var buffer = new byte[4];

            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }

            if (minValue == maxValue)
            {
                return minValue;
            }

            long diff = maxValue - minValue;

            while (true)
            {
                this.rng.GetBytes(buffer);
                var rand = BitConverter.ToUInt32(buffer, 0);

                var remainder = Max % diff;

                if (rand < Max - remainder)
                {
                    return (int)(minValue + (rand % diff));
                }
            }
        }

        private HashAlgorithm GetCryptoServiceProvider()
        {
            return new SHA256CryptoServiceProvider();
        }
    }
}
