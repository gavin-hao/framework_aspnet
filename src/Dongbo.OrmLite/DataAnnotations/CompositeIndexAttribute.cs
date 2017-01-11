using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 复合索引
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class CompositeIndexAttribute : ServiceStack.DataAnnotations.CompositeIndexAttribute
    {
        /// <summary>
        /// 复合索引
        /// </summary>
        public CompositeIndexAttribute() : base() { }

        /// <summary>
        /// 复合索引
        /// </summary>
        /// <param name="fieldNames">索引的字段</param>
        public CompositeIndexAttribute(params string[] fieldNames) : base(fieldNames) { }

        /// <summary>
        /// 复合索引
        /// </summary>
        /// <param name="unique">是否是唯一索引</param>
        /// <param name="fieldNames">索引的字段</param>
        public CompositeIndexAttribute(bool unique, params string[] fieldNames) : base(unique, fieldNames) { }
    }
}