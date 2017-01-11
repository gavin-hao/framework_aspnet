using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dongbo.Configuration;
using System.Xml.Serialization;
namespace Dongbo.DFS.Oss
{
    [Serializable]
    [XmlRoot("OssConfig")]
    public class OssConfig : BaseConfig<OssConfig>
    {
        public const string SectionName = "OssConfig";

        /// <summary>
        /// <your access id>
        /// </summary>
        [XmlElement("accessId")]
        public string AccessId { get; set; }
        /// <summary>
        /// <your access key>
        /// </summary>
        [XmlElement("accessKey")]
        public string AccessKey { get; set; }
        /// <summary>
        /// 访问域名
        /// 以北京节点为例:http://oss-cn-beijing.aliyuncs.com;
        /// 北京节点内网地址：http://oss-cn-beijing-internal.aliyuncs.com
        /// </summary>
        [XmlElement("endPoint")]
        public string EndPoint { get; set; }
        /// <summary>
        /// 内网访问OSS 访问域名
        /// </summary>
        /// <remarks>
        /// 只有使用ECS实例用户才能够通过OSS内网地址进行访问,使用内网只能访问与ECS实例所在区域相同的bucket，而如果访问的bucket与ECS实例不在同一个区域，只能使用bucket所在区域的公网域名。
        /// </remarks>
        [XmlElement("endPointInternal")]
        public string EndPointInternal { get; set; }
        /// <summary>
        /// 设置请求发生错误时最大的重试次数
        /// </summary>
        [XmlElement("maxErrorRetry")]
        public int MaxErrorRetry
        {
            get { return maxErrorRetry; }
            set { maxErrorRetry = value; }
        }private int maxErrorRetry = 3;

        /// <summary>
        /// 设置连接超时时间
        /// </summary>
        [XmlElement("connectionTimeout")]
        public int ConnectionTimeout
        {
            get { return connectionTimeout; }
            set { connectionTimeout = value; }
        }
        private int connectionTimeout = 300;

        /// <summary>
        /// work enviroment ;default 0
        /// 0:production , 1:testing , 2:development
        /// when enviroment=0 then oss connect with internal endpoint;else external endpoint; 
        /// </summary>
        [XmlElement("environment")]
        public int Environment
        {
            get { return environment; }
            set { environment = value; }
        }
        private int environment = 0;
        [XmlElement("cname")]
        public string CnameDomain
        {
            get;
            set;
        }
    }
}
