using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 自定义字段
    /// 可用于在生成创建表的DDL语句中指定字段的声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomFieldAttribute : ServiceStack.DataAnnotations.CustomFieldAttribute
    {
        /// <summary>
        /// 可用于在生成创建表的DDL语句中指定字段的声明
        /// </summary>
        /// <param name="sql">定义语句</param>
        public CustomFieldAttribute(string sql) : base(sql) { }
    }
}