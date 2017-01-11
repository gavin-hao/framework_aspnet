using Dongbo.DFS.Oss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dongbo.Common;
using Dongbo.Common.Util;
using System.Web;
using System.Collections.Specialized;
using Dongbo.Logging;
using Aliyun.OSS;

namespace Dongbo.DFS.Oss
{
    public class OssProvider : IStorageProvider
    {

        //private static readonly string default_application = DfsConfig.Instance.Applications.FirstOrDefault().ApplicationName;
        private static readonly LogWrapper log = new LogWrapper();
        public OssProvider()
        {

        }
        private DfsPath _GetDfsPath(string path)
        {
            return new DfsPath(path);
        }




        public string AddFile(string path, Stream data, NameValueCollection userMetadata)
        {

            var dfs = _GetDfsPath(path);
            var bucket = dfs.Bucket;
            var skey = dfs.path.Replace("\\", "/").ToLower().TrimStart('/');
            string applicationName = dfs.ApplicationName;
            var metadata = new ObjectMetadata();
            if (!String.IsNullOrEmpty(applicationName))
            {
                var conf = DfsConfig.Instance[applicationName];
                if (conf != null)
                {
                    metadata.CacheControl = conf.CacheControl;
                    metadata.ContentEncoding = conf.ContentEncoding;
                    if (conf.Expires > 0)
                    {
                        metadata.AddHeader(HttpHeaders.Expires, DateTime.Now.AddSeconds(conf.Expires));
                    }
                }
            }

            if (userMetadata != null && userMetadata.Count > 0)
            {
                string cc = userMetadata[HttpHeaders.CacheControl];
                if (!string.IsNullOrEmpty(cc))
                    metadata.CacheControl = userMetadata[HttpHeaders.CacheControl];
                var ce = userMetadata[HttpHeaders.ContentEncoding];
                if (!string.IsNullOrEmpty(ce))
                    metadata.ContentEncoding = ce;

                var cd = userMetadata[HttpHeaders.ContentDisposition];
                if (!string.IsNullOrEmpty(cd))
                    metadata.ContentDisposition = cd;

                var ct = userMetadata[HttpHeaders.ContentType];
                if (!string.IsNullOrEmpty(ct))
                    metadata.ContentType = ct;

                var md5 = userMetadata[HttpHeaders.ContentMd5];
                if (!string.IsNullOrEmpty(md5))
                    metadata.ContentMd5 = md5;


                foreach (string key in userMetadata.AllKeys)
                {
                    var v = userMetadata[key];


                    metadata.UserMetadata.Add(key, v);
                }
            }
            metadata.ContentLength = data.Length;
            if (String.IsNullOrEmpty(metadata.ContentType))
            {
                var contentType = Mime.Lookup(skey);
                metadata.ContentType = contentType;
            }
            try
            {
                var result = OssService.Instance.PutObject(bucket, skey, data, metadata);
                return result.ETag;
            }
            catch (Exception ex)
            {
                log.Error(new Exception(path, ex));
                return String.Empty;
            }

        }
        public Stream GetFileStream(string path)
        {
            var dfs = _GetDfsPath(path);
            try
            {
                return OssService.Instance.GetObject(dfs.Bucket, dfs.path).Content;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }

        }

        public void DeleteFile(string path)
        {
            var dfs = _GetDfsPath(path);
            try
            {
                OssService.Instance.DeleteObject(dfs.Bucket, dfs.path);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

        }

        public List<string> GetFiles(string dirPath)
        {
            var dfs = _GetDfsPath(dirPath);
            try
            {
                var list = OssService.Instance.ListObjects(dfs.Bucket, dfs.path, null);
                var result = new List<string>();
                foreach (var v in list)
                {
                    result.Add(v.Key);
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }

        }

        public MBFileInfo GetFileInfo(string path)
        {
            var dfs = _GetDfsPath(path);
            ObjectMetadata metadata = null;
            try
            {
                metadata = OssService.Instance.GetObjectMetadata(dfs.Bucket, dfs.path);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new MBFileInfo();
            }

            MBFileInfo info = new MBFileInfo();
            var ali = "";
            metadata.UserMetadata.TryGetValue("Aliases", out ali);
            if (!string.IsNullOrEmpty(ali))
            {
                try
                {
                    var a = SerializeHelper.DeserializeJson<string[]>(ali);
                    info.Aliases = a;
                }
                catch { }
                if (info.Aliases == null || info.Aliases.Length == 0)
                {
                    info.Aliases = new string[] { ali };
                }
            }
            info.Ext = Path.GetExtension(dfs.path);
            info.Name = Path.GetFileName(dfs.path);
            info.Metadata = new NameValueCollection();
            info.UploadDate = metadata.LastModified;
            info.LastModified = metadata.LastModified;
            info.Size = metadata.ContentLength;
            info.ContentType = metadata.ContentType;
            info.ETag = metadata.ETag;


            foreach (var item in metadata.UserMetadata)
            {
                info.Metadata.Add(item.Key, item.Value);
            }
            info.Metadata.Add(HttpHeaders.CacheControl, metadata.CacheControl);
            info.Metadata.Add(HttpHeaders.ContentDisposition, metadata.ContentDisposition);
            info.Metadata.Add(HttpHeaders.ContentEncoding, metadata.ContentEncoding);
            info.Metadata.Add(HttpHeaders.Expires, metadata.ExpirationTime.ToString());
            return info;
        }


        public void CopyFile(string sourcePath, string targetPath, NameValueCollection metadata)
        {
            var sdfs = _GetDfsPath(sourcePath);
            var tdfs = _GetDfsPath(targetPath);

            var newMetadata = new ObjectMetadata();
            foreach (string item in metadata)
            {
                newMetadata.AddHeader(item, metadata[item]);
            }
            try
            {
                OssService.Instance.CopyObject(sdfs.Bucket, sdfs.path, tdfs.Bucket, tdfs.path, newMetadata);

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public string GetPresignedUrl(string path)
        {
            var dfs = _GetDfsPath(path);

            return OssService.Instance.GeneratePresignedUri(dfs.Bucket, dfs.path, 3600, SignHttpMethod.Get).ToString();
        }


        public string GetHttpUrl(string path)
        {
            var dfs = _GetDfsPath(path);
            var url = string.Format("http://{0}.{1}/{2}", dfs.Bucket, OssConfig.Instance.EndPoint.Replace("http://", ""), dfs.path);
            return url;
        }


        public bool DoesFileExist(string path)
        {
            var dfs = _GetDfsPath(path);
            return OssService.Instance.DoesObjectExist(dfs.Bucket, dfs.path);
        }


        public void DowloadFile(string filePath, string localFilePath)
        {
            var dfs = _GetDfsPath(filePath);
            var bucket = dfs.Bucket;
            var skey = dfs.path.Replace("\\", "/").ToLower().TrimStart('/');
            OssService.Instance.DownloadObject(bucket, skey, localFilePath);
        }
    }
}
