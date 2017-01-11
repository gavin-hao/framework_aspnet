using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PostCreateTableAttribute : ServiceStack.DataAnnotations.PostCreateTableAttribute
    {
        public PostCreateTableAttribute(string sql) : base(sql)
        {
        }
    }
}