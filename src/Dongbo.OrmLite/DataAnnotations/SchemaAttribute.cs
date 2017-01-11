using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 架构
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaAttribute : ServiceStack.DataAnnotations.SchemaAttribute
    {
        public SchemaAttribute(string name) : base(name)
        {
        }
    }
}