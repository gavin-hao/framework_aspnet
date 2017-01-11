using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 自增特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AutoIncrementAttribute : ServiceStack.DataAnnotations.AutoIncrementAttribute
    {
    }
}