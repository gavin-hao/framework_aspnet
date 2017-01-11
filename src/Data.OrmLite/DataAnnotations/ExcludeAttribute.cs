using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExcludeAttribute : ServiceStack.DataAnnotations.ExcludeAttribute
    {
        public ExcludeAttribute(Feature feature) : base((ServiceStack.Feature)feature)
        {
        }
    }

    [Flags]
    public enum Feature
    {
        All = 2147483647,
        Csv = 256,
        CustomFormat = 1024,
        Html = 512,
        Json = 8,
        Jsv = 32,
        Markdown = 2048,
        Metadata = 1,
        MsgPack = 16384,
        None = 0,
        PredefinedRoutes = 2,
        ProtoBuf = 8192,
        Razor = 4096,
        RequestInfo = 4,
        Soap = 192,
        Soap11 = 64,
        Soap12 = 128,
        Xml = 16,
    }
}