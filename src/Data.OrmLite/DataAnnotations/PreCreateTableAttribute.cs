using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PreCreateTableAttribute : ServiceStack.DataAnnotations.PreCreateTableAttribute
    {
        public PreCreateTableAttribute(string sql) : base(sql)
        {
        }
    }
}