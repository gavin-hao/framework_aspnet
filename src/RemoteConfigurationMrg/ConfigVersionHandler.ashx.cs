using System;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.IO;
using Beisen.Configuration;
using System.Configuration;

namespace Beisen.RemoteConfigurationManager
{
    public class ConfigVersionHandler : IHttpHandler
    {
        private const string NoAppPath = "General";

        static private string GetDownloadUrl(string sectionName, string applicationName, int major, int minor, int configType)
        {
            string folder = string.Empty;
            switch (configType)
            {
                case 1:
                    {
                        folder = ConfigurationManager.AppSettings["httpPublishFolder"];
                        break;
                    }
                case 2:
                    {
                        folder = ConfigurationManager.AppSettings["httpResourceFolder"];
                        break;
                    }
            }
            return folder + "/" + sectionName + "/" + applicationName + "/" + major + "/" + sectionName + "." + minor;
        }

        static string GetLocalFolderName(string sectionName, int major)
        {
            string baseFolder = ConfigurationManager.AppSettings["publishFolder"];
            return baseFolder + " \\" + sectionName + "\\" + major;
        }

        private int GetLastVersion(string sectionName, string applicationName, int major, out int configType, out bool ExitAppConfig)
        {
            ExitAppConfig = false;
            string folder = ConfigurationManager.AppSettings["publishFolder"];

            folder = Path.Combine(folder, sectionName);
            folder = Path.Combine(folder, applicationName);
            folder = Path.Combine(folder, major.ToString());

            if (!Directory.Exists(folder))
            {
                folder = ConfigurationManager.AppSettings["resourceFolder"];

                folder = Path.Combine(folder, sectionName);
                folder = Path.Combine(folder, applicationName);
                folder = Path.Combine(folder, major.ToString());
                if (!Directory.Exists(folder))
                {
                    configType = 0;
                    return -1;
                }
                else
                {
                    configType = 2;
                }
            }
            else
            {
                configType = 1;
            }

            int maxMinor = -1;
            string[] minorVersions = Directory.GetFiles(folder);
            foreach (string minorVersion in minorVersions)
            {
                string fileName = minorVersion.Substring(folder.Length + 1);
                string[] strs = fileName.Split(".".ToCharArray());
                int minor;
                if (int.TryParse(strs[strs.Length - 1], out minor))
                {
                    ExitAppConfig = true;
                    maxMinor = Math.Max(maxMinor, minor);
                }
            }
            if (maxMinor == -1)
            {
                return -1;
            }
            else
            {
                return maxMinor;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.InputStream.Length == 0)
            {
                return;
            }
            XmlSerializer xser = new XmlSerializer(typeof(RemoteConfigSectionCollection));
            RemoteConfigSectionCollection rcc = (RemoteConfigSectionCollection)xser.Deserialize(context.Request.InputStream);

            RemoteConfigSectionCollection ret = new RemoteConfigSectionCollection();
            int configType = 0;
            bool ExitAppConfig = false;
            foreach (RemoteConfigSectionParam param in rcc)
            {
                if (!string.IsNullOrEmpty(rcc.Application))
                {
                    int minor = GetLastVersion(param.SectionName, rcc.Application, param.MajorVersion, out configType, out ExitAppConfig);
                    if (minor > param.MinorVersion)
                    {
                        string url = GetDownloadUrl(param.SectionName, rcc.Application, param.MajorVersion, minor, configType);
                        ret.AddSection(param.SectionName, param.MajorVersion, minor, url);
                        continue;
                    }
                }
                //如果指定了应用程序，且应用程序有配置文件，就不再处理默认配置文件
                if (!ExitAppConfig)
                {
                    int minor2 = GetLastVersion(param.SectionName, NoAppPath, param.MajorVersion, out configType, out ExitAppConfig);
                    if (minor2 > param.MinorVersion)
                    {
                        string url = GetDownloadUrl(param.SectionName, NoAppPath, param.MajorVersion, minor2, configType);
                        ret.AddSection(param.SectionName, param.MajorVersion, minor2, url);
                    }
                }
            }

            context.Response.ContentType = "text/xml";
            xser.Serialize(context.Response.OutputStream, ret);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
