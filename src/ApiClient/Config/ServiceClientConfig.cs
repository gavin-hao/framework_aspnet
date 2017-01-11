using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;
using Dongbo.Configuration;
using System.Collections;

namespace Dongbo.ApiClient
{
    [Serializable]
    [XmlRoot("ServiceClientSetting")]
    public class ServiceClientSetting : BaseConfig<ServiceClientSetting>
    {
        [XmlIgnore]
        public static readonly string ServiceNamePrefix = "";//MbRESTServices_

        [XmlArray("Services")]
        [XmlArrayItem("Service", Type = typeof(Service))]
        public List<Service> Services = new List<Service>();
        [XmlIgnore]
        public int Count
        {
            get
            {
                if (Services != null)
                {
                    return Services.Count;
                }
                return 0;
            }
        }
        [XmlIgnore]
        public Service this[int i]
        {
            get
            {
                if (Services != null)
                {
                    return Services[i];
                }
                return null;
            }
        }
        [XmlIgnore]
        public Service this[string name]
        {
            get
            {
                if (Services != null)
                {
                    return Services.SingleOrDefault(c => c.Name == ServiceNamePrefix + name);
                }
                return null;
            }
        }
        public IEnumerator GetEnumerator()
        {
            return Services.GetEnumerator();
        }
    }
    [Serializable]
    [XmlRoot("Service")]
    public class Service
    {
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("EndPoint")]
        public string EndPoint;
        [XmlAttribute("Enable")]
        public bool Enable;
        [XmlElement("Behavior", Type=typeof(Behavior))]
        public Behavior Behavior;
    }
    [Serializable]
    public class Behavior
    {
        public Behavior()
        {
            Timeout = 30000L;
        }
        //[XmlAttribute("Name")]
        //public string Name;

        [XmlAttribute("Timeout")]
        public long Timeout;

        [XmlAttribute("CompletionOption")]
        public string CompletionOption;

        [XmlAttribute("TransferMode")]
        public string TransferMode;

        [XmlAttribute("MaxBufferSize")]
        public int MaxBufferSize;

        [XmlAttribute("MaxReceivedMessageSize")]
        public int MaxReceivedMessageSize;

        [XmlAttribute("MessageEncoding")]
        public string MessageEncoding;

        [XmlAttribute("TextEncoding")]
        public string TextEncoding;

        [XmlAttribute("Ssl")]
        public bool Ssl { get; set; }
    }

}
