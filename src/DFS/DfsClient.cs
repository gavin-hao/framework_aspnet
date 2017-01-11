using Dongbo.Common.Util;
using Dongbo.DFS.Oss;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.DFS
{
    public class DfsClient : IDfsProvider
    {
        private IStorageProvider _service = null;
        private DfsApplication _appConfig = null;
        public DfsClient(string application)
        {
            if (string.IsNullOrEmpty(application))
            {
                throw new ArgumentNullException("application", "application name is required!");
            }
            _service = AppCollections.Instance[application];
            if (_service == null)
            {
                throw new ArgumentException("application is unsupported! Please check your configuration file [DfsConfig.config], ensure that you have configured the specified application in Applications section");
            }
            _appConfig = DfsConfig.Instance[application];
        }


        public string AddFile(string path, byte[] fileData)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }

            using (MemoryStream ms = new MemoryStream(fileData))
            {
                return AddFile(path, ms);
            };
        }
        public string AddFile(string path, Stream data)
        {
            return AddFile(path, data, new NameValueCollection());
        }

        public string AddFile(string path, Stream data, NameValueCollection userMetadata)
        {
            //path = DfsHelper.ParseToDfsPath(path, _appConfig.BucketName);
            path = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, path).ToString();
            return _service.AddFile(path, data, userMetadata);
        }

        public string AddFile(string path, byte[] data, string[] aliases)
        {
            var metadata = new NameValueCollection();
            var aliJson = SerializeHelper.SerializeJson(aliases);
            metadata.Add("Aliases", aliJson);
            using (MemoryStream ms = new MemoryStream(data))
            {
                return AddFile(path, ms, metadata);
            }
        }


        public byte[] GetFile(string path)
        {

            using (Stream s = GetFileStream(path))
            {
                if (s == null)
                {
                    return null;
                }
                MemoryStream stmMemory = new MemoryStream();
                byte[] buffer = new byte[64 * 1024];
                int i;
                while ((i = s.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stmMemory.Write(buffer, 0, i);
                }
                byte[] arraryByte = stmMemory.ToArray();

                stmMemory.Close();

                return arraryByte;
            }
        }

        public Stream GetFileStream(string path)
        {
            path = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, path).ToString();
            return _service.GetFileStream(path);
        }

        public void DeleteFile(string path)
        {
            path = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, path).ToString();
            _service.DeleteFile(path);
        }
        /// <summary>
        /// 获取文件夹下的所有文件路径
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public List<string> GetFiles(string dirPath)
        {
            dirPath = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, dirPath).ToString();
            return _service.GetFiles(dirPath);
        }

        public MBFileInfo GetFileInfo(string path)
        {
            path = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, path).ToString();
            return _service.GetFileInfo(path);
        }

        public void CopyFile(string sourcePath, string targetPath)
        {
            CopyFile(sourcePath, targetPath, new NameValueCollection());
        }

        public void CopyFile(string sourcePath, string targetPath, NameValueCollection metadata)
        {
            sourcePath = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, sourcePath).ToString();
            targetPath = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, targetPath).ToString();

            NameValueCollection meta = new NameValueCollection(metadata);
            meta.Add(HttpHeaders.CacheControl, _appConfig.CacheControl);
            meta.Add(HttpHeaders.ContentEncoding, _appConfig.ContentEncoding);
            if (_appConfig.Expires > 0)
                meta.Add(HttpHeaders.Expires, DateTime.Now.AddSeconds(_appConfig.Expires).ToString());
            _service.CopyFile(sourcePath, targetPath, metadata);
        }


        public string GetPresignedUrl(string path)
        {
            path = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, path).ToString();
            return _service.GetPresignedUrl(path);
        }
        public string GetHttpUrl(string path)
        {
            path = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, path).ToString();
            return _service.GetHttpUrl(path);
        }



        public bool DoesFileExist(string path)
        {
            path = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, path).ToString();
            return _service.DoesFileExist(path);
        }


        public void DowloadFile(string filePath, string localFilePath)
        {
            filePath = new DfsPath(_appConfig.ApplicationName, _appConfig.BucketName, filePath).ToString();
            _service.DowloadFile(filePath, localFilePath);
        }
    }


    internal class AppCollections
    {
        #region singleton
        private static readonly Dictionary<string, IStorageProvider> _appProviders = new Dictionary<string, IStorageProvider>();
        private static AppCollections instance = new AppCollections();
        private AppCollections()
        {
            DfsConfig.ConfigChanged += DfsConfig_ConfigChanged;
            //_appProviders.Add("common", ProviderSet.Instance["Oss"]);
        }
        public static AppCollections Instance { get { return instance; } }
        #endregion

        void DfsConfig_ConfigChanged(object sender, EventArgs e)
        {
            _appProviders.Clear();
        }
        public IStorageProvider this[string app]
        {
            get
            {
                if (_appProviders.ContainsKey(app))
                    return _appProviders[app];
                else
                {
                    var p = InitAppProvider(app);
                    if (p != null)
                        _appProviders[app] = p;
                    return p;
                }
            }
        }
        private IStorageProvider InitAppProvider(string appName)
        {
            var apps = DfsConfig.Instance.Applications;
            if (apps == null || apps.Count() == 0)
                return null;
            var theApp = apps.First(p => p.ApplicationName.ToLower() == appName.ToLower());
            if (theApp == null)
                return null;
            var provider = ProviderSet.Instance[theApp.Provider];
            return provider;
        }
    }
    internal class ProviderSet
    {

        private static object locker = new Object();
        private static readonly Dictionary<string, IStorageProvider> mappers = new Dictionary<string, IStorageProvider>();

        private static ProviderSet instance = new ProviderSet();
        private ProviderSet()
        {
            mappers.Add("Oss", new OssProvider());
        }
        public static ProviderSet Instance { get { return instance; } }
        public IStorageProvider this[string key]
        {
            get
            {
                lock (locker)
                {
                    if (mappers.ContainsKey(key))
                    {
                        return mappers[key];
                    }
                    else
                    {
                        var newProvider = GenerateProviderInstance(key);
                        if (newProvider == null)
                        {
                            throw new NotSupportedException(string.Format("the dfs provider type:{0}  is unsupported!", key));
                        }
                        else
                        {
                            mappers[key] = newProvider;
                            return newProvider;
                        }

                    }
                }
            }

        }




        private IStorageProvider GenerateProviderInstance(string typeName)
        {
            var provider = ProviderLoader.LoadProvider(typeName);
            if (provider == null)
            {
                throw new TypeLoadException(String.Format("can not load the provider type[{0}]", typeName));
            }


            return provider;
        }




    }
    internal class ProviderLoader
    {

        public static IStorageProvider LoadProvider(string typename)
        {
            Type providerType = FindProvider(typename);
            if (providerType == null)
                return null;
            var instance = Activator.CreateInstance(providerType) as IStorageProvider;
            return instance;
        }
        private static Type FindProvider(string typename)
        {
            var ass = LoadAssemblies();
            if (ass.Length == 0)
                return null;
            foreach (var a in ass)
            {
                var t = a.GetType(typename);
                if (t != null)
                    return t;
            }
            return null;
        }
        private static Assembly[] LoadAssemblies()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            var assfiles = Directory.GetFiles(baseDir, "*.dll", SearchOption.AllDirectories);
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var f in assfiles)
            {

                try
                {
                    var ass = Assembly.LoadFile(f);
                    var providers = ass.GetTypes().Where(p => p.IsClass && p.IsPublic && typeof(IServiceProvider).IsAssignableFrom(p) && p.FullName != "Dongbo.DFS.DfsClient");
                    if (providers != null && providers.Count() > 0)
                    {
                        assemblies.Add(ass);
                    }
                }
                catch (Exception e)
                {

                }

            }
            return assemblies.ToArray();
        }
    }

}
