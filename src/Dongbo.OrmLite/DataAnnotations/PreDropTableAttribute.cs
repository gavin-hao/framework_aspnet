using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PreDropTableAttribute : ServiceStack.DataAnnotations.PreDropTableAttribute
    {
        public PreDropTableAttribute(string sql) : base(sql)
        {
        }
    }
}