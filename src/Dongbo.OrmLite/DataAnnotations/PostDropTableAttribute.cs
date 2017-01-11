using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PostDropTableAttribute : ServiceStack.DataAnnotations.PostDropTableAttribute
    {
        public PostDropTableAttribute(string sql) : base(sql)
        {
        }
    }
}