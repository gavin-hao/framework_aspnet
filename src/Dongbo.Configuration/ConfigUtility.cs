using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Diagnostics;


namespace Dongbo.Configuration
{
    public sealed class ConfigUtility
    {
        public static bool IsWebApplication
        {
            get
            {
                string AppVirtualPath = System.Web.HttpRuntime.AppDomainAppVirtualPath;
                if (!string.IsNullOrEmpty(AppVirtualPath))
                {
                    /* 
                     * Use the root web.config. This did check where the request was and 
                     * load that web.config. In longhorn we cannot call HttpContext.Request in 
                     * the Application Start. This throws an exception. Changed to always pull
                     * the root web.config
                     * */
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static string baseDomain;
        public static string BaseDomain
        {
            get
            {
                if (baseDomain == null)
                {
                    baseDomain = System.Configuration.ConfigurationManager.AppSettings["baseDomain"];
                    if (baseDomain == null)
                        baseDomain = "dongboke.com";
                }
                return baseDomain;
            }
        }

        /// <summary>
        /// Build absolute url based on BaseDomain
        /// </summary>
        /// <param name="host">host name without domain</param>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string BuildUrl(string host, string relativeUrl)
        {
            if (string.IsNullOrEmpty(host))
                return string.Concat("http://", BaseDomain, relativeUrl);
            else
                return string.Concat("http://", host, ".", BaseDomain, relativeUrl);
        }


        public static IPAddress LocalIPAdddress
        {
            get
            {
                string hostName = Dns.GetHostName();
                string ip = Dns.Resolve(hostName).AddressList[0].ToString();

                return IPAddress.Parse(ip);
            }
        }

        public static NetworkEnvironment CurrentEnvironment
        {
            get
            {
                string strEnv = System.Configuration.ConfigurationManager.AppSettings["environment"];
                if (!string.IsNullOrEmpty(strEnv))
                {
                    try
                    {
                        NetworkEnvironment env = (NetworkEnvironment)Enum.Parse(typeof(NetworkEnvironment), strEnv);
                        return env;
                    }
                    catch
                    {
                    }
                }

                try
                {
                    IPHostEntry iph = Dns.GetHostEntry(Dns.GetHostName());
                    if (iph.AddressList != null)
                    {
                        foreach (IPAddress address in iph.AddressList)
                        {
                            byte[] bytes = address.GetAddressBytes();
                            if (bytes[0] == 10 && bytes[1] == 22)
                                return NetworkEnvironment.Production;
                        }
                    }
                }
                catch
                {

                }

                return NetworkEnvironment.Development;
            }
        }



        public static bool IsProd
        {
            get
            {
                return CurrentEnvironment == NetworkEnvironment.Production;
            }
        }
      
        public static bool IsTesting
        {
            get
            {
                return CurrentEnvironment == NetworkEnvironment.Testing;               
            }
        }

        public static bool IsStage
        {
            get
            {
                return CurrentEnvironment == NetworkEnvironment.Labs;
            }
        }

        public static bool IsDemo
        {
            get
            {
                return CurrentEnvironment == NetworkEnvironment.Demo;
            }
        }

        static string executablePath;

        public static string ExecutablePath
        {
            get
            {
                if (executablePath == null)
                {
                    if (IsWebApplication)
                    {
                        executablePath = System.AppDomain.CurrentDomain.BaseDirectory;
                    }
                    else
                    {
                        System.Reflection.Assembly ass = System.Reflection.Assembly.GetEntryAssembly();
                        if (ass != null)
                            executablePath = ass.Location;
                        else
                            executablePath = System.AppDomain.CurrentDomain.BaseDirectory;
                    }
                }
                return executablePath;
            }
        }


        static string applicationName;
        public static string ApplicationName
        {
            get
            {
                if (applicationName == null)
                {
                    applicationName = System.Configuration.ConfigurationManager.AppSettings["applicationName"];
                    if (applicationName == null)
                        applicationName = EncodedExecutablePath;
                }
                return applicationName;
            }
        }

        static string rootConfigFolder;
        public static string RootConfigFolder
        {
            get
            {
                if (rootConfigFolder == null)
                {
                    rootConfigFolder = System.Configuration.ConfigurationManager.AppSettings["configRoot"];
                    if (rootConfigFolder == null)
                        rootConfigFolder = DefaultRootConfigFolder;
                }
                return rootConfigFolder;
            }
        }

        static string rootLogFolder;
        public static string RootLogFolder
        {
            get
            {
                if (rootLogFolder == null)
                {
                    rootLogFolder = System.Configuration.ConfigurationManager.AppSettings["logRoot"];
                    if (rootLogFolder == null)
                        rootLogFolder = DefaultRootLogFolder;
                }
                return rootLogFolder;
            }
        }


        public const string DefaultRootConfigFolder = "c:\\MB.configs";
        public const string DefaultRootLogFolder = "c:\\MB.logfiles";

        public static string DefaultApplicationConfigFolder = Path.GetFullPath(Path.Combine(RootConfigFolder, ApplicationName));
        public static string DefaultApplicationLogFolder = Path.GetFullPath(Path.Combine(RootLogFolder, ApplicationName));

        public static string Combine(string folder, string file)
        {
            return Path.GetFullPath(Path.Combine(folder, file));
        }

        static bool IsLetterOrNumber(char i)
        {
            return ((i >= 'A' && i <= 'Z') || (i >= 'a' && i <= 'z') || (i >= '0' && i <= '9'));
        }

        static string EncodeToPath(string path)
        {
            char[] chs = path.ToCharArray();
            for (int i = 0; i < path.Length; i++)
            {
                if (!IsLetterOrNumber(chs[i]))
                {
                    chs[i] = '_';
                }
            }
            return new string(chs);
        }

        static string encodedExecutablePath;

        public static string EncodedExecutablePath
        {
            get
            {
                if (encodedExecutablePath == null)
                    encodedExecutablePath = EncodeToPath(ExecutablePath);
                return encodedExecutablePath;
            }
        }

        public static int GetIntValue(XmlReader reader, string name, int defaultValue)
        {
            int val;
            if (!int.TryParse(reader.GetAttribute(name), out val))
                val = defaultValue;
            return val;
        }

        public static bool GetBoolValue(XmlReader reader, string name, bool defaultValue)
        {
            bool val;
            if (!bool.TryParse(reader.GetAttribute(name), out val))
                val = defaultValue;
            return val;
        }

        public static string GetStringValue(XmlReader reader, string name, string defaultValue)
        {
            string val;
            val = reader.GetAttribute(name);
            if (val == null)
                val = defaultValue;
            return val;
        }

        public static void DumpStack(TextWriter writer)
        {
            StackTrace trace = new StackTrace();
            foreach (StackFrame frame in trace.GetFrames())
            {
                string file = frame.GetFileName();
                int line = frame.GetFileLineNumber();
                int column = frame.GetFileColumnNumber();
                string methodName = frame.GetMethod().Name;
                string clsName = frame.GetMethod().DeclaringType.FullName;
                writer.WriteLine(clsName + "." + methodName + "," + file + ":" + line);
            }
        }
    }

    public enum NetworkEnvironment
    {
        Production,        
        Development,
        Testing,
        Labs,
        Demo
    }
}
