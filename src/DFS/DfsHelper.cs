using Dongbo.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.DFS
{
    public class DfsHelper
    {
        //public static string ParseToUrl(string)
    }

    public class DfsPath
    {
        public DfsPath(string dfsPathString)
        {
            if (string.IsNullOrEmpty(dfsPathString))
                throw new ArgumentNullException("dfsPathString");
            var path = dfsPathString.Split(':');
            if (path.Length != 3)
                throw new ArgumentException(string.Format("{0} is not a correct DfsPath format", dfsPathString));
            populate(path[0], path[1], path[2]);
        }

        public DfsPath(string application, string bucket, string path)
        {
            populate(application, bucket, path);
        }
        private void populate(string application, string bucket, string path)
        {
            this.Bucket = bucket.Replace(":", "");
            this.path = path.Replace("\\", "/").Replace(":", "").TrimStart('/');
            this.ApplicationName = application.Replace(":", "");
        }
        public string Bucket { get; set; }
        public string path { get; set; }
        public string ApplicationName { get; set; }
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.ApplicationName, this.Bucket, this.path);
        }
    }
}
