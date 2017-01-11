using Dongbo.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using System.Collections;
namespace Dongbo.Redis
{
    [Serializable]
    [XmlRoot("RedisConfig")]
    public class RedisConfiguration : BaseConfig<RedisConfiguration>
    {
        public const string SectionName = "RedisConfig";
        [XmlArray("clusters"), XmlArrayItem("cluster")]
        public Cluster[] Clusters { get; set; }

        [XmlIgnore]
        public Cluster this[string name]
        {

            get
            {
                if (this.Clusters == null || this.Clusters.Length == 0)
                {
                    return null;
                }
                return Clusters.FirstOrDefault(p => string.Equals(name, p.ClusterName, StringComparison.OrdinalIgnoreCase));
            }
        }


    }

    [Serializable]
    public class Cluster
    {
        [XmlAttribute("name")]
        public string ClusterName;

        [XmlAttribute("hosts")]
        public string Hosts;

        /// <summary>
        /// supported options
        /// <see cref="https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Configuration.md"/>
        /// </summary>
        [XmlAttribute("options")]//allowAdmin=False,connectTimeout=5000,connectRetry=3,defaultDatabase=0,password=123456
        public string Options;

        public override string ToString()
        {
            string v = Hosts;
            if (!string.IsNullOrEmpty(Options))
                v += ("," + Options.TrimStart(','));
            return v;
        }
    }
}
