using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 排除
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : ServiceStack.DataAnnotations.IgnoreAttribute
    {
    }
}