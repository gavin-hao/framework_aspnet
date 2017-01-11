using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
   
    public class CustomDataProtectorTokenProvider<TUser, TKey> : IUserTokenProvider<TUser, TKey>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="protector"></param>
        public CustomDataProtectorTokenProvider(IDataProtector protector)
        {
            if (protector == null)
            {
                throw new ArgumentNullException("protector");
            }
            Protector = protector;
            TokenLifespan = TimeSpan.FromDays(2);
        }

        /// <summary>
        ///     IDataProtector for the token
        /// </summary>
        public IDataProtector Protector { get; private set; }

        /// <summary>
        ///     Lifespan after which the token is considered expired
        /// </summary>
        public TimeSpan TokenLifespan { get; set; }

        /// <summary>
        ///     Generate a protected string for a user
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateAsync(string purpose, UserManager<TUser, TKey> manager, TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var ms = new MemoryStream();
            using (var writer = ms.CreateWriter())
            {
                writer.Write(DateTimeOffset.UtcNow);
                writer.Write(Convert.ToString(user.Id));
                writer.Write(purpose ?? "");
                string stamp = null;
                if (manager.SupportsUserSecurityStamp)
                {
                    stamp = await manager.GetSecurityStampAsync(user.Id);
                }
                writer.Write(stamp ?? "");
            }
            var protectedBytes = Protector.Protect(ms.ToArray());
            var token = Convert.ToBase64String(protectedBytes, Base64FormattingOptions.None);
            return Base64ToNormalString(token);
        }

        /// <summary>
        ///     Return false if the token is not valid
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser, TKey> manager, TUser user)
        {
            try
            {
                var frombase64Token = Convert.FromBase64String(ToBase64String(token));
                var unprotectedData = Protector.Unprotect(frombase64Token);
                var ms = new MemoryStream(unprotectedData);
                using (var reader = ms.CreateReader())
                {
                    var creationTime = reader.ReadDateTimeOffset();
                    var expirationTime = creationTime + TokenLifespan;
                    if (expirationTime < DateTimeOffset.UtcNow)
                    {
                        return false;
                    }

                    var userId = reader.ReadString();
                    if (!String.Equals(userId, Convert.ToString(user.Id)))
                    {
                        return false;
                    }
                    var purp = reader.ReadString();
                    if (!String.Equals(purp, purpose))
                    {
                        return false;
                    }
                    var stamp = reader.ReadString();
                    if (reader.PeekChar() != -1)
                    {
                        return false;
                    }

                    if (manager.SupportsUserSecurityStamp)
                    {
                        var expectedStamp = await manager.GetSecurityStampAsync(user.Id);
                        return stamp == expectedStamp;
                    }
                    return stamp == "";
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception e)
            {
                // Do not leak exception
            }
            return false;
        }

        /// <summary>
        ///     Returns true if the provider can be used to generate tokens for this user
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> IsValidProviderForUserAsync(UserManager<TUser, TKey> manager, TUser user)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///     This provider no-ops by default when asked to notify a user
        /// </summary>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task NotifyAsync(string token, UserManager<TUser, TKey> manager, TUser user)
        {
            return Task.FromResult(0);
        }

        private string Base64ToNormalString(string base64String)
        {
            return base64String.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
        private string ToBase64String(string normalString)
        {
            return Pad(normalString.Replace('-', '+').Replace('_', '/'));
        }
        private string Pad(string text)
        {
            var padding = 3 - ((text.Length + 3) % 4);
            if (padding == 0)
            {
                return text;
            }
            return text + new string('=', padding);
        }
    }

    internal static class StreamExtensions
    {
        internal static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        public static BinaryReader CreateReader(this Stream stream)
        {
            return new BinaryReader(stream, DefaultEncoding, true);
        }

        public static BinaryWriter CreateWriter(this Stream stream)
        {
            return new BinaryWriter(stream, DefaultEncoding, true);
        }

        public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader)
        {
            return new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
        }

        public static void Write(this BinaryWriter writer, DateTimeOffset value)
        {
            writer.Write(value.UtcTicks);
        }
    }
}
