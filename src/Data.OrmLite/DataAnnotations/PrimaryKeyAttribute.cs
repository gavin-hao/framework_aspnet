using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : ServiceStack.DataAnnotations.PrimaryKeyAttribute
    {
    }
}