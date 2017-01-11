using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.DFS
{
    public class MBFileInfo
    {
        public string Name { get; set; }

        public string Ext { get; set; }

        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime UploadDate { get; set; }
        public string ETag { get; set; }
        public string[] Aliases { get; set; }

        public NameValueCollection Metadata { get; set; }

        public string ContentType { get; set; }
    }
}
