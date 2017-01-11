using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 复合主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class CompositeKeyAttribute : ServiceStack.DataAnnotations.CompositeKeyAttribute
    {
        /// <summary>
        /// 复合主键
        /// </summary>
        public CompositeKeyAttribute() : base() { }

        /// <summary>
        /// 复合主键
        /// </summary>
        /// <param name="fieldNames">主键的字段</param>
        public CompositeKeyAttribute(params string[] fieldNames) : base(fieldNames) { }
    }
}