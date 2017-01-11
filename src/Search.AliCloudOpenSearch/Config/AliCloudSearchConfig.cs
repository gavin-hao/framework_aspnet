using Dongbo.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dongbo.Search.AliCloudOpenSearch
{
    [Serializable]
    [XmlRoot("AliCloudSearchConfig")]
    public class AliCloudSearchConfig : BaseConfig<AliCloudSearchConfig>
    {
        public const string SectionName = "AliCloudSearchConfig";

        [XmlElement("accessKey")]
        public string AccessKey { get; set; }

        [XmlElement("accessSecret")]
        public string AccessSecret { get; set; }

        [XmlElement("host")]
        public string Host { get; set; }

        [XmlArray("applications"), XmlArrayItem("app")]
        public Application[] Applications { get; set; }


        [XmlIgnore]
        public Application this[string name]
        {

            get
            {
                if (this.Applications == null || this.Applications.Length == 0)
                {
                    return null;
                }
                return Applications.FirstOrDefault(p => string.Equals(name, p.Name, StringComparison.OrdinalIgnoreCase));
            }
        }


    }

    [Serializable]
    public class Application
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("indexName")]
        public string IndexName { get; set; }
        [XmlAttribute("host")]
        public string Host { get; set; }
        [XmlAttribute("host_internal")]
        public string HostInternal { get; set; }
        [XmlAttribute("host_vpc")]
        public string HostVPC { get; set; }
        [XmlAttribute("description")]
        public string Description { get; set; }

    }


}
