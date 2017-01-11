using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 外键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : ServiceStack.DataAnnotations.ForeignKeyAttribute
    {
        public ForeignKeyAttribute(Type type) : base(type)
        {
        }
    }
}