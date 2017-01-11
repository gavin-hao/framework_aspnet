using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.DFS
{
    public interface IDfsProvider : IStorageProvider
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        string AddFile(string path, byte[] fileData);
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        string AddFile(string path, Stream data);
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="userMetadata">自定义元数据信息</param>
        /// <returns></returns>
        string AddFile(string path, Stream data, NameValueCollection userMetadata);


        ///// <summary>
        ///// 保存文件, 兼容老的mongodb系统，请尽量不要使用该方法
        ///// </summary>
        ///// <param name="path"></param>
        ///// <param name="data"></param>
        ///// <param name="aliases"></param>
        ///// <returns></returns>
        //string AddFile(string path, Stream data, string[] aliases);


        byte[] GetFile(string path);

        void CopyFile(string sourcePath, string targetPath);

        
    }
}
