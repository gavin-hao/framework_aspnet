﻿using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Owin.Security.AesDataProtectionProvider
{
    internal class AesDataProtector : IDataProtector
    {
        private readonly byte[] _key;

        public AesDataProtector(string key)
        {


            using (var sha = SHACrypto())
                _key = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
        public byte[] Protect(byte[] data)
        {
            byte[] dataHash;

            using (var sha = SHACrypto())
            {
                dataHash = sha.ComputeHash(data);

            }

            using (var aesAlg = AesCrypto())
            {
                aesAlg.Key = _key;
                aesAlg.GenerateIV();

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, 16);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var bwEncrypt = new BinaryWriter(csEncrypt))
                    {
                        bwEncrypt.Write(dataHash);
                        bwEncrypt.Write(data.Length);
                        bwEncrypt.Write(data);
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            using (var aesAlg = AesCrypto())
            {
                aesAlg.Key = _key;

                using (var msDecrypt = new MemoryStream(protectedData))
                {
                    var iv = new byte[16];
                    msDecrypt.Read(iv, 0, 16);
                    aesAlg.IV = iv;

                    if (aesAlg.Key == null)
                        throw new InvalidOperationException("Key is null");

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var brDecrypt = new BinaryReader(csDecrypt))
                    {
                        var signature = brDecrypt.ReadBytes(32);
                        var len = brDecrypt.ReadInt32();
                        var data = brDecrypt.ReadBytes(len);
                        byte[] dataHash;

                        using (var sha = SHACrypto())
                            dataHash = sha.ComputeHash(data);

                        if (!dataHash.SequenceEqual(signature))
                            throw new SecurityException("Signature does not match the computed hash");

                        return data;
                    }
                }
            }
        }

        private AesCryptoServiceProvider AesCrypto()
        {
            return new AesCryptoServiceProvider();
        }

        private SHA256CryptoServiceProvider SHACrypto()
        {
            return new SHA256CryptoServiceProvider();
        }
    }
}
