using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 行版本
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RowVersionAttribute : ServiceStack.DataAnnotations.RowVersionAttribute
    {
    }
}