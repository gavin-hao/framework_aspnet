using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using System.IO;
using System.Globalization;
using Aliyun.OSS.Common;

namespace Dongbo.DFS.Oss
{
    public class OssService
    {
        private OssService()
        {
            OssConfig.ConfigChanged += OssConfig_ConfigChanged;
        }
        private static OssService instance = new OssService();
        public static OssService Instance
        {
            get
            {
                return instance;
            }
        }

        void OssConfig_ConfigChanged(object sender, EventArgs e)
        {
            //when ossconfig changed then reset ossClient;
            _client = null;
        }


        private OssClient _client = null;
        /// <summary>
        ///  initialize a OssClient instance, connected by internal 
        /// </summary>
        public OssClient Client
        {
            get
            {
                if (_client == null)
                {
                    if (OssConfig.Instance.Environment == 0)//use internal endpoint
                        _client = GetClient(true);
                    else
                    {
                        _client = GetClient(false);
                    }
                }

                return _client;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isInternal">使用内网访问OSS? default:false</param>
        /// <returns></returns>
        internal OssClient GetClient(bool isInternal = false)
        {
            ClientConfiguration _conf = new ClientConfiguration();
            _conf.MaxErrorRetry = OssConfig.Instance.MaxErrorRetry;     //设置请求发生错误时最大的重试次数
            _conf.ConnectionTimeout = OssConfig.Instance.ConnectionTimeout;  //设置连接超时时间
            var url = isInternal ? OssConfig.Instance.EndPointInternal : OssConfig.Instance.EndPoint;
            var endpoint = new Uri(url);
            var _client = new OssClient(endpoint, OssConfig.Instance.AccessId, OssConfig.Instance.AccessKey, _conf);

            return _client;
        }
        public void CreateBucket(string bucketName)
        {

            Client.CreateBucket(bucketName);

        }

        public IEnumerable<Bucket> ListBuckets()
        {
            var buckets = Client.ListBuckets();
            return buckets;
        }
        public void DeleteBucket(string bucketName)
        {

            Client.DeleteBucket(bucketName);

        }

        public bool DoesBucketExist(string bucketName)
        {
            return Client.DoesBucketExist(bucketName);
        }
        public void CreateEmptyFolder(string bucketName, string folderName)
        {
            var key = folderName.TrimStart('/').TrimEnd('/') + '/';
            using (MemoryStream memStream = new MemoryStream())
            {
                PutObjectResult ret = Client.PutObject(bucketName, key, memStream);
            }
        }
        public void SetBucketCors(string bucketName, string allowedOrigin, string allowedMethod, string allowedHeader, string exposeHeader)
        {

            var req = new SetBucketCorsRequest(bucketName);
            var r1 = new CORSRule();
            //指定允许跨域请求的来源
            r1.AddAllowedOrigin(allowedOrigin);
            //指定允许的跨域请求方法(GET/PUT/DELETE/POST/HEAD)
            r1.AddAllowedMethod(allowedMethod);
            //控制在OPTIONS预取指令中Access-Control-Request-Headers头中指定的header是否允许。
            r1.AddAllowedHeader(allowedHeader);
            //指定允许用户从应用程序中访问的响应头
            r1.AddExposeHeader(exposeHeader);
            req.AddCORSRule(r1);
            Client.SetBucketCors(req);
        }

        /// <summary>
        /// OSS是按使用收费的服务，为了防止用户在OSS上的数据被其他人盗链，OSS支持基于HTTP header中表头字段referer的防盗链方法
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="refererList"> </param>
        /// <returns></returns>
        /// <![CDATA[
        ///  //refererList.Add(" http://www.aliyun.com");
        ///refererList.Add(" http://www.*.com");
        ///refererList.Add(" http://www.?.aliyuncs.com");
        /// ]]>
        public RefererConfiguration SetBucketReferer(string bucketName, List<string> refererList)
        {

            if (refererList == null || refererList.Count == 0)
            {
                return null;
            }
            var srq = new SetBucketRefererRequest(bucketName, refererList);
            Client.SetBucketReferer(srq);

            var rc = Client.GetBucketReferer(bucketName);
            return rc;

        }
        public RefererConfiguration GetBucketReferer(string bucketName)
        {

            return Client.GetBucketReferer(bucketName);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="rules"></param>
        /// <![CDATA[
        ///  LifecycleRule lcr1 = new LifecycleRule()
        ///   {
        ///       ID = "delete obsoleted files",
        ///      Prefix = "obsoleted/",
        ///      Status = RuleStatus.Enabled,
        ///      ExpriationDays = 3
        ///  };
        /// ]]>
        public void SetBucketLifecycle(string bucketName, LifecycleRule[] rules)
        {
            if (rules.Length == 0)
                return;
            var setBucketLifecycleRequest = new SetBucketLifecycleRequest(bucketName);

            for (var i = 0; i < rules.Length; i++)
            {
                setBucketLifecycleRequest.AddLifecycleRule(rules[i]);
            }

            Client.SetBucketLifecycle(setBucketLifecycleRequest);

        }

        public IList<LifecycleRule> GetBucketLifecycle(string bucketName)
        {
            var rule = Client.GetBucketLifecycle(bucketName);
            return rule;
        }

        public void SetBucketAcl(string bucketName, CannedAccessControlList policy)
        {
            Client.SetBucketAcl(bucketName, policy);
        }
        public AccessControlList GetBucketAcl(string bucketName)
        {
            var acl = Client.GetBucketAcl(bucketName);
            return acl;
        }
        //Object
        public string GetObjectSignedUrl(string bucketName, string key, Dictionary<string, string> queryParams, Dictionary<string, string> userMataData)
        {
            var policyConds = new PolicyConditions();
            var client = this.Client;
            var metadata = client.GetObjectMetadata(bucketName, key);
            var etag = metadata.ETag;
            var req = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get);
            // Set optional properties(be blind to them usually) 
            if (queryParams != null)
            {
                foreach (var item in queryParams)
                {
                    req.AddQueryParam(item.Key, item.Value);
                }
            }
            if (userMataData != null)
            {
                foreach (var item in userMataData)
                {
                    req.AddQueryParam(item.Key, item.Value);
                }
            }

            req.ContentType = "text/html";
            req.ContentMd5 = etag;
            req.ResponseHeaders.CacheControl = "No-Cache";
            req.ResponseHeaders.ContentEncoding = "utf-8";
            req.ResponseHeaders.ContentType = "text/html";
            var uri = client.GeneratePresignedUri(req);
            return uri.ToString();
        }

        public Uri GeneratePresignedUri(string bucketName, string key, int expiration_s, SignHttpMethod method)
        {
            if (expiration_s == 0)
                expiration_s = 3600;
            var req = new GeneratePresignedUriRequest(bucketName, key, method);
            req.Expiration = DateTime.Now.AddSeconds(expiration_s);
            //req.ContentType = "text/html";
            //req.ContentMd5 = etag;
            //req.ResponseHeaders.CacheControl = "No-Cache";
            //req.ResponseHeaders.ContentEncoding = "utf-8";
            //req.ResponseHeaders.ContentType = "text/html";
            var client = GetClient();
            var uri = client.GeneratePresignedUri(req);
            return uri;
        }
        public bool DoesObjectExist(string bucketName, string key)
        {
            try
            {
                var exist = Client.DoesObjectExist(bucketName, key);
                return exist;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }
        public ObjectMetadata GetObjectMetadata(String bucketName, string key)
        {
            var metadata = Client.GetObjectMetadata(bucketName, key);
            return metadata;
        }
        public byte[] GetObjectBytes(string bucketName, string key)
        {
            var o = Client.GetObject(bucketName, key);
            int len = (int)o.Metadata.ContentLength;
            var buf = new byte[len];
            using (var requestStream = o.Content)
            {

                requestStream.Read(buf, 0, len);

            }
            return buf;
        }
        public OssObject GetObject(string bucketName, string key)
        {
            var o = Client.GetObject(bucketName, key);
            return o;
        }
        public OssObject GetObject(Uri uri)
        {
            var o = Client.GetObject(uri);
            return o;
        }

        public PutObjectResult PutObject(string bucketName, string key, Stream content)
        {
            return Client.PutObject(bucketName, key, content);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="metadata">Cache-Control 、 Content-Disposition 、Content-Encoding 、 Expires </param>
        /// <returns></returns>
        public PutObjectResult PutObject(string bucketName, string key, Stream content, ObjectMetadata metadata)
        {
            return Client.PutObject(bucketName, key, content, metadata);
        }
        public void DeleteObject(string bucketName, string key)
        {
            Client.DeleteObject(bucketName, key);
        }
        public DeleteObjectsResult DeleteObjects(string bucketName, IList<string> keys)
        {
            var request = new DeleteObjectsRequest(bucketName, keys, false);
            return Client.DeleteObjects(request);
        }
        /// <summary>
        /// 方法我们可以拷贝一个Object
        /// 使用该方法copy的object必须小于1G，否则会报错。若object大于1G，使用后文的Upload Part Copy
        /// </summary>
        /// <param name="sourceBucket"></param>
        /// <param name="sourceKey"></param>
        /// <param name="targetBucket"></param>
        /// <param name="targetKey"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public CopyObjectResult CopyObject(string sourceBucket, string sourceKey, string targetBucket, string targetKey, ObjectMetadata metadata)
        {
            var req = new CopyObjectRequest(sourceBucket, sourceKey, targetBucket, targetKey)
            {
                NewObjectMetadata = metadata
            };
            var ret = Client.CopyObject(req);
            return ret;
        }
        /// <summary>
        /// 列出Bucket中的Object
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="prefix"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public List<OssObjectSummary> ListObjects(string bucketName, string prefix, string delimiter)
        {
            List<OssObjectSummary> objectSummaries = new List<OssObjectSummary>();
            ObjectListing result = null;
            string nextMarker = string.Empty;
            do
            {
                var listObjectsRequest = new ListObjectsRequest(bucketName)
                {
                    Delimiter = delimiter,
                    Prefix = prefix,
                    Marker = nextMarker,
                    MaxKeys = 100
                };
                result = Client.ListObjects(listObjectsRequest);
                objectSummaries.AddRange(result.ObjectSummaries);

                nextMarker = result.NextMarker;
            } while (result.IsTruncated);
            return objectSummaries;
        }

        public void DownloadObject(string bucketName, string fileKey, string localFilePath)
        {

            using (var fileStream = new FileStream(localFilePath, FileMode.OpenOrCreate))
            {
                var bufferedStream = new BufferedStream(fileStream);
                var objectMetadata = Client.GetObjectMetadata(bucketName, fileKey);
                var fileLength = objectMetadata.ContentLength;
                const int partSize = 1024 * 1024 * 10;

                var partCount = CalPartCount(fileLength, partSize);

                for (var i = 0; i < partCount; i++)
                {
                    var startPos = partSize * i;
                    var endPos = partSize * i + (partSize < (fileLength - startPos) ? partSize : (fileLength - startPos)) - 1;
                    GetObject(bufferedStream, startPos, endPos, localFilePath, bucketName, fileKey);
                }
                bufferedStream.Flush();
            }
        }
        /// <summary>
        /// 计算下载的块数
        /// </summary>
        /// <param name="fileLength"></param>
        /// <param name="partSize"></param>
        /// <returns></returns>
        private int CalPartCount(long fileLength, long partSize)
        {
            var partCount = (int)(fileLength / partSize);
            if (fileLength % partSize != 0)
            {
                partCount++;
            }
            return partCount;
        }

        /// <summary>
        /// 分块下载
        /// </summary>
        /// <param name="bufferedStream"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="localFilePath"></param>
        /// <param name="bucketName"></param>
        /// <param name="fileKey"></param>
        private void GetObject(BufferedStream bufferedStream, long startPos, long endPos, String localFilePath, String bucketName, String fileKey)
        {
            Stream contentStream = null;
            try
            {
                var getObjectRequest = new GetObjectRequest(bucketName, fileKey);
                getObjectRequest.SetRange(startPos, endPos);
                var ossObject = Client.GetObject(getObjectRequest);
                byte[] buffer = new byte[1024 * 1024];
                var bytesRead = 0;
                bufferedStream.Seek(startPos, SeekOrigin.Begin);
                contentStream = ossObject.Content;
                while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bufferedStream.Write(buffer, 0, bytesRead);
                }
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Dispose();
                }
            }
        }

    }
}
