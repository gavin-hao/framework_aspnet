using System;

namespace Dongbo.OrmLite.DataAnnotations
{
    /// <summary>
    /// 数据库字段默认值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DefaultAttribute : ServiceStack.DataAnnotations.DefaultAttribute
    {
        /// <summary>
        /// 数据库字段默认值
        /// </summary>
        public DefaultAttribute(string defaultValue) : base(defaultValue) { }

        /// <summary>
        /// 数据库字段默认值
        /// </summary>
        public DefaultAttribute(double doubleValue) : base(doubleValue) { }

        /// <summary>
        /// 数据库字段默认值
        /// </summary>
        public DefaultAttribute(int intValue) : base(intValue) { }

        /// <summary>
        /// 数据库字段默认值
        /// </summary>
        public DefaultAttribute(Type defaultType, string defaultValue) : base(defaultType, defaultValue) { }
    }
}