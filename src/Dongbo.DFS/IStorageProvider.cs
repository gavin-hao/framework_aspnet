using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.DFS
{
    public interface IStorageProvider
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data">a stream ;you should close the stream by yourself when added</param>
        /// <param name="applicationName"></param>
        /// <param name="userMetadata"></param>
        /// <returns></returns>
        string AddFile(string path, Stream data, NameValueCollection userMetadata);
        Stream GetFileStream(string path);
        List<string> GetFiles(string dirPath);
        MBFileInfo GetFileInfo(string path);
        void DeleteFile(string path);
        string GetPresignedUrl(string path);
        void CopyFile(string sourcePath, string targetPath, NameValueCollection metadata);
        /// <summary>
        /// parse file key to  a http url 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetHttpUrl(string path);
        bool DoesFileExist(string path);

        void DowloadFile(string filePath, string localFilePath);

    }
}
