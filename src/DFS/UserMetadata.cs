using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.DFS
{
    internal static class HttpHeaders
    {
        public const string Authorization = "Authorization";

        public const string CacheControl = "Cache-Control";

        public const string ContentDisposition = "Content-Disposition";

        public const string ContentEncoding = "Content-Encoding";

        public const string ContentLength = "Content-Length";

        public const string ContentMd5 = "Content-MD5";

        public const string ContentType = "Content-Type";

        public const string Date = "Date";

        public const string Expires = "Expires";

        public const string ETag = "ETag";

        public const string LastModified = "Last-Modified";

        public const string Range = "Range";

        public static bool IsDefined(string header)
        {
            var fileds = typeof(HttpHeaders).GetFields();

            foreach (var filed in fileds)
            {
                var v = (string)filed.GetValue("");
                if (string.IsNullOrEmpty(v))
                {
                    if (v == header)
                        return true;
                }

            }
            return false;
        }
    }
    public class MetadataCollection
    {
        private const string DefaultObjectContentType = "application/octet-stream";
        private readonly IDictionary<string, string> _userMetadata =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly IDictionary<string, object> _metadata =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public const string Aes256ServerSideEncryption = "AES256";

        /// <summary>
        /// 获取用户自定义的元数据。
        /// </summary>
        /// <remarks>
        /// OSS内部保存用户自定义的元数据时，会以x-oss-meta-为请求头的前缀。
        /// 但用户通过该接口处理用户自定义元数据里，不需要加上前缀“x-oss-meta-”。
        /// 同时，元数据字典的键名是不区分大小写的，并且在从服务器端返回时会全部以小写形式返回，
        /// 即使在设置时给定了大写字母。比如键名为：MyUserMeta，通过GetObjectMetadata接口
        /// 返回时键名会变为：myusermeta。
        /// </remarks>
        public IDictionary<string, string> UserMetadata
        {
            get { return _userMetadata; }
        }

        /// <summary>
        /// 获取Last-Modified请求头的值，表示Object最后一次修改的时间。
        /// </summary>
        public DateTime LastModified
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.LastModified)
                    ? (DateTime)_metadata[HttpHeaders.LastModified] : DateTime.MinValue;
            }
            internal set
            {
                _metadata[HttpHeaders.LastModified] = value;
            }
        }

        /// <summary>
        /// 获取Expires请求头，表示Object的过期时间。
        /// 如果Object没有定义过期时间，则返回null。
        /// </summary>
        public DateTime ExpirationTime
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.Expires)
                     ? (DateTime)_metadata[HttpHeaders.Expires] : DateTime.MinValue;
            }
            internal set
            {
                _metadata[HttpHeaders.Expires] = value;
            }
        }



        /// <summary>
        /// 获取或设置Content-Type请求头，表示Object内容的类型，为标准的MIME类型。
        /// </summary>
        public string ContentType
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ContentType)
                    ? _metadata[HttpHeaders.ContentType] as string : null;
            }
            set
            {
                _metadata[HttpHeaders.ContentType] = value;
            }
        }

        /// <summary>
        /// 获取或设置Content-Encoding请求头，表示Object内容的编码方式。
        /// </summary>
        public string ContentEncoding
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ContentEncoding)
                    ? _metadata[HttpHeaders.ContentEncoding] as string : null;
            }
            set
            {
                _metadata[HttpHeaders.ContentEncoding] = value;
            }
        }

        /// <summary>
        /// 获取或设置Cache-Control请求头，表示用户指定的HTTP请求/回复链的缓存行为。
        /// </summary>
        public string CacheControl
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.CacheControl)
                    ? _metadata[HttpHeaders.CacheControl] as string : null;
            }
            set
            {
                _metadata[HttpHeaders.CacheControl] = value;
            }
        }

        /// <summary>
        /// 获取Content-Disposition请求头，表示MIME用户代理如何显示附加的文件。
        /// </summary>
        public string ContentDisposition
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ContentDisposition)
                    ? _metadata[HttpHeaders.ContentDisposition] as string : null;
            }
            set
            {
                _metadata[HttpHeaders.ContentDisposition] = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值表示与Object相关的hex编码的128位MD5摘要。
        /// </summary>
        public string ETag
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ETag)
                    ? _metadata[HttpHeaders.ETag] as string : null;
            }
            set
            {
                _metadata[HttpHeaders.ETag] = value;
            }
        }




        public void AddHeader(string key, object value)
        {
            _metadata.Add(key, value);
        }

        /// <summary>
        /// Populates the request header dictionary with the metdata and user metadata.
        /// </summary>
        /// <param name="requestHeaders"></param>
        //internal void Populate(IDictionary<string, string> requestHeaders)
        //{
        //    foreach (var entry in _metadata)
        //        requestHeaders.Add(entry.Key, entry.Value.ToString());

        //    if (!requestHeaders.ContainsKey(HttpHeaders.ContentType))
        //        requestHeaders.Add(HttpHeaders.ContentType, DefaultObjectContentType);

        //    foreach (var entry in _userMetadata)
        //        requestHeaders.Add(OssHeaders.OssUserMetaPrefix + entry.Key, entry.Value);
        //}
    }
}
