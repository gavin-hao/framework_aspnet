using Dongbo.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dongbo.DFS
{
    [Serializable]
    [XmlRoot("DfsConfig")]
    public class DfsConfig : BaseConfig<DfsConfig>
    {
        public const string SectionName = "DfsConfig";
        [XmlIgnore]
        private string cacheControl = "max-age";
        [XmlElement("CacheControl")]
        public string CacheControl
        {
            get { return cacheControl; }
            set { cacheControl = value; }
        }
        [XmlIgnore]
        private string contentEncoding = "utf-8";
        [XmlElement("ContentEncoding")]
        public string ContentEncoding
        {
            get { return contentEncoding; }
            set { contentEncoding = value; }
        }

        [XmlArray("Applications"), XmlArrayItem(Type = typeof(DfsApplication), ElementName = "Application")]
        public List<DfsApplication> Applications
        {
            get;
            set;
        }
        [XmlIgnore]
        public DfsApplication this[string name]
        {
            get
            {
                return Applications.First(p => string.Equals(name, p.ApplicationName, StringComparison.OrdinalIgnoreCase));
            }
        }
        public IEnumerator GetEnumerator()
        {
            return Applications.GetEnumerator();
        }
    }
    [Serializable]
    public class DfsApplication
    {
        [XmlAttribute("Name")]
        public string ApplicationName { get; set; }
        [XmlAttribute("Provider")]
        public string Provider { get { return _provider; } set { _provider = value; } }
        [XmlIgnore]
        private string _provider = "Oss";

        [XmlAttribute("BucketName")]
        public string BucketName { get; set; }
        [XmlIgnore]
        private string cacheControl = "max-age";
        [XmlAttribute("CacheControl")]
        public string CacheControl
        {
            get { return cacheControl; }
            set { cacheControl = value; }
        }

        [XmlAttribute("Acl")]
        public int AccessContolList
        {
            get;
            set;
        }

        [XmlIgnore]
        private string contentEncoding = "utf-8";
        [XmlAttribute("ContentEncoding")]
        public string ContentEncoding
        {
            get { return contentEncoding; }
            set { contentEncoding = value; }
        }
        [XmlAttribute("Expires")]
        public long Expires
        {
            get;
            set;
        }
    }

}
